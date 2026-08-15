# Project rules for Claude

## What this is

ProcessKiller is a small console application that reads a list of process names from `Config.xml`
and kills every running process with one of those names. It has no user interface, no arguments and
no output on success. It is shipped as an Inno Setup installer, it is **not** published as a NuGet
package: no `GeneratePackageOnBuild`, no push script.

One solution `src/ProcessKiller.sln` with exactly one project:

- `src/ProcessKiller/ProcessKiller.csproj`, `OutputType` `Exe`, `TargetFramework` `net9.0`.

Layout inside `src/ProcessKiller`:

- `Program.cs`: everything the application does. `Main` locates `Config.xml` next to the executable,
  deserializes it and kills the matching processes. The private helpers `ImportConfiguration` and
  `CreateObjectFromString<T>` do one thing each, keep new logic in that shape.
- `Config.cs`: the root of the configuration file, a single `List<Process> Processes`.
- `Process.cs`: one entry of that list, `Name` and `FullName`. The type name collides with
  `System.Diagnostics.Process`, see the quirks below.
- `Config.xml`: the shipped default configuration, copied to the output directory with
  `CopyToOutputDirectory=Always`. It contains one dummy entry `Test` / `Test.exe`.
- `GlobalUsings.cs`: all usings of the project.
- `License.txt`, `Readme.txt`, `ProcessKiller.ico`: content for the installer. `License.txt` and
  `Readme.txt` are also copied to the output directory, the icon becomes the `ApplicationIcon`.

`Setup` holds the Inno Setup script `ProcessKiller-Setup.iss`, the publish helper
`build-setup-files.bat` and the built installer `ProcessKiller-Setup.exe`. The installer is
**tracked** even though `.gitignore` excludes `*.exe`, it was added with `git add -f` and has to be
added that way again on every release.

Repository root: `README.md` (the only user documentation, note the uppercase spelling, the sibling
repositories use `Readme.md`), `Changelog.md`, `License.txt` (MIT), `.gitignore` and
`.gitattributes`. There is no `Updating.md`, no `HowToUse.md`, no screenshots and no `.github`
folder.

## Build

```powershell
dotnet build src/ProcessKiller.sln -c Release
```

- Single target framework `net9.0` in the one project, no multi-targeting, no `RuntimeIdentifiers`.
  Nothing in the code is Windows specific, but killing Windows processes is the entire point and the
  installer is Windows only.
- All build properties live directly in `src/ProcessKiller/ProcessKiller.csproj`. There is **no**
  `Directory.Build.props` in this repository.
- `TreatWarningsAsErrors` is enabled, so every warning breaks the build, NuGet warnings (`NU****`)
  from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.0.8-1` for the first
  commit after tag `1.0.7`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/ProcessKiller.sln -c Release --source https://api.nuget.org/v3/index.json`.
- There is no test project. A behaviour change is verified by running the published executable
  against a `Config.xml` and checking which processes are gone afterwards. Never claim a run
  happened without running it.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace.
- XML doc comments on every type and every member, private members included, no exceptions.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into `GlobalUsings.cs`, inside the existing `#pragma warning disable
  IDE0065` block, never at the top of a file. The editorconfig requires usings inside the namespace
  (`csharp_using_directive_placement=inside_namespace:warning`), which global usings cannot satisfy,
  that is what the pragma is for. Do not add other pragmas. The comment text in that block is German
  because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`). `Program` is `static`, so there is
  currently no place in the code where that shows.
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **Two types named `Process`.** The configuration model `ProcessKiller.Process` shadows
  `System.Diagnostics.Process` inside the namespace, so `Program.cs` has to call
  `System.Diagnostics.Process.GetProcessesByName` fully qualified. Renaming the model would change
  the element name in `Config.xml` and break every installed configuration.
- **`FullName` is never read.** `Config.xml` carries `Test.exe` in it and the property is
  serialized, but only `Name` reaches `GetProcessesByName`. The property stays because removing it
  would make existing configuration files fail to deserialize.
- **`GetProcessesByName` wants the name without `.exe`.** That is why `Name` and `FullName` exist
  side by side. A configuration that puts `Test.exe` into `Name` silently matches nothing.
- **The application is silent on success.** No console output, no exit code other than 0 unless it
  crashes. Judge a run by which processes are gone, not by stdout.
- **The installer is tracked despite `.gitignore`.** `Setup/ProcessKiller-Setup.exe` is excluded by
  the `*.exe` rule and was force added. Every release needs `git add -f Setup/ProcessKiller-Setup.exe`.
- **`Config.xml` is overwritten on every build.** `CopyToOutputDirectory=Always`, so a configuration
  edited in `bin` is gone after the next build. Edit the file in the project directory.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository. There is no pipeline file here.
- **`src/ProcessKiller.sln.DotSettings`** is tracked and holds nothing but a ReSharper user
  dictionary (`H_00E4mmer`). Leave it alone.
- **`.gitattributes` sets `* text=auto`**, every rule of the Visual Studio template below it is
  commented out. Any binary file that must not be normalized needs its own rule.
- **`Setup/build-setup-files.bat` deletes all `bin` and `obj` folders** below `src` before it
  publishes, and ends with `pause`. Running it from a script needs `cd /d` into `Setup` first,
  because the `cd ..\src` in the first line is relative to the start directory.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.0.8.0 (2026-08-15)** : Short description.`
3. Set `MyAppVersion` in `Setup/ProcessKiller-Setup.iss` to the same four part version.
4. Commit that.
5. Tag the commit with the plain version number, no `v` prefix (`1.0.8`, `1.0.7`, ...). The existing
   tags are lightweight tags, create new ones the same way. The tag has to exist **before** the
   installer is built, otherwise GitVersion burns a prerelease version into the shipped executable.
6. Run `Setup/build-setup-files.bat`, then compile `Setup/ProcessKiller-Setup.iss` with
   `ISCC.exe`.
7. `git add -f Setup/ProcessKiller-Setup.exe`, commit as `Updated setup.`.
8. Push the commits and the tag.

The version in the `Changelog.md` has four parts (`1.0.8.0`), the tag has three (`1.0.8`).
GitVersion turns the tag into the assembly version, so an untagged commit produces something like
`1.0.8-1+Branch.master.Sha...`.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
