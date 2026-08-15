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
    /// The name of the configuration file, expected next to the executable.
    /// </summary>
    private const string ConfigFileName = "Config.xml";

    /// <summary>
    /// The configuration service.
    /// </summary>
    private static readonly IConfigService ConfigService = new ConfigService();

    /// <summary>
    /// The process service.
    /// </summary>
    private static readonly IProcessService ProcessService = new ProcessService();

    /// <summary>
    /// The main method.
    /// </summary>
    /// <returns>0 if the configuration was read, 1 if it was not.</returns>
    private static int Main()
    {
        try
        {
            var fileName = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
            var config = ConfigService.ImportConfiguration(fileName);
            ProcessService.KillProcesses(config);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return 1;
        }
    }
}
