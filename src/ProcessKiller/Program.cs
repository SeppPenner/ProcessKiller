// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller;

/// <summary>
/// The main program.
/// </summary>
public static class Program
{
    /// <summary>
    /// The configuration.
    /// </summary>
    private static Config config = new();

    /// <summary>
    /// The main method.
    /// </summary>
    private static void Main()
    {
        try
        {
            var location = Assembly.GetExecutingAssembly().Location;
            config = ImportConfiguration(Path.Combine(Directory.GetParent(location)?.FullName ?? string.Empty, "Config.xml")) ?? new();

            foreach (var process in config.Processes.SelectMany(p => System.Diagnostics.Process.GetProcessesByName(p.Name)))
            {
                process.Kill();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
        }
    }

    /// <summary>
    /// Imports the configuration.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>A new <see cref="Config"/> object.</returns>
    private static Config? ImportConfiguration(string fileName)
    {
        var xDocument = XDocument.Load(fileName);
        return CreateObjectFromString<Config?>(xDocument);
    }

    /// <summary>
    /// Creates a object from a <see cref="string"/>.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="xDocument">The X document.</param>
    /// <returns>A new object of type <see cref="T"/>.</returns>
    private static T? CreateObjectFromString<T>(XDocument xDocument)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        return (T?)xmlSerializer.Deserialize(new StringReader(xDocument.ToString()));
    }
}
