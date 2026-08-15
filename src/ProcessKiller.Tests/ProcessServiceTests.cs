// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessServiceTests.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to test the <see cref="ProcessService" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller.Tests;

/// <summary>
/// A class to test the <see cref="ProcessService"/> class.
/// </summary>
[TestClass]
public class ProcessServiceTests
{
    /// <summary>
    /// The process service under test.
    /// </summary>
    private readonly IProcessService processService = new ProcessService();

    /// <summary>
    /// Checks whether an empty configuration is a no operation.
    /// </summary>
    [TestMethod]
    public void KillProcessesWithAnEmptyConfigurationKillsNothing()
    {
        var killed = this.processService.KillProcesses(new Config());

        Assert.AreEqual(0, killed);
    }

    /// <summary>
    /// Checks whether a name that matches no running process is a no operation instead of an exception.
    /// </summary>
    [TestMethod]
    public void KillProcessesWithAnUnknownNameKillsNothing()
    {
        var config = TestDataProvider.GetConfig($"ThisProcessDoesNotExist{Guid.NewGuid():N}");

        var killed = this.processService.KillProcesses(config);

        Assert.AreEqual(0, killed);
    }

    /// <summary>
    /// Checks whether an empty name is skipped. Without the filter this would ask the operating system for every
    /// process with an empty name.
    /// </summary>
    [TestMethod]
    public void KillProcessesWithAnEmptyNameKillsNothing()
    {
        var killed = this.processService.KillProcesses(TestDataProvider.GetConfig(string.Empty, "   "));

        Assert.AreEqual(0, killed);
    }

    /// <summary>
    /// Checks whether a running process is actually killed. The target is a copy of the command processor under a
    /// name that exists nowhere else on the machine, so that the test can never hit a foreign process.
    /// </summary>
    [TestMethod]
    public void KillProcessesKillsARunningProcess()
    {
        var directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(directory);
        var processName = $"PkTestTarget{Guid.NewGuid():N}";
        System.Diagnostics.Process? target = null;

        try
        {
            var fileName = Path.Combine(directory, $"{processName}.exe");
            File.Copy(Path.Combine(Environment.SystemDirectory, "cmd.exe"), fileName);

            target = StartTarget(fileName);
            Assert.AreEqual(1, System.Diagnostics.Process.GetProcessesByName(processName).Length, "The target process did not start.");

            var killed = this.processService.KillProcesses(TestDataProvider.GetConfig(processName));

            Assert.AreEqual(1, killed);
            Assert.IsTrue(target.WaitForExit(10000), "The target process was not killed.");
            Assert.AreEqual(0, System.Diagnostics.Process.GetProcessesByName(processName).Length);
        }
        finally
        {
            KillIfStillRunning(target);
            Directory.Delete(directory, true);
        }
    }

    /// <summary>
    /// Starts the given executable as a command processor that waits for input and therefore stays alive.
    /// </summary>
    /// <param name="fileName">The file name of the executable.</param>
    /// <returns>The started <see cref="System.Diagnostics.Process"/>.</returns>
    private static System.Diagnostics.Process StartTarget(string fileName)
    {
        var startInfo = new ProcessStartInfo(fileName)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        var process = System.Diagnostics.Process.Start(startInfo) ?? throw new InvalidOperationException("The target process could not be started.");
        Assert.IsTrue(SpinWait.SpinUntil(() => System.Diagnostics.Process.GetProcessesByName(process.ProcessName).Length > 0, 10000), "The target process did not show up.");
        return process;
    }

    /// <summary>
    /// Kills the given process if the test left it behind.
    /// </summary>
    /// <param name="process">The process, may be <c>null</c> if the start already failed.</param>
    private static void KillIfStillRunning(System.Diagnostics.Process? process)
    {
        if (process is null)
        {
            return;
        }

        using (process)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit(10000);
                }
            }
            catch (Exception)
            {
                // The process is gone already, which is exactly what the test wanted.
            }
        }
    }
}
