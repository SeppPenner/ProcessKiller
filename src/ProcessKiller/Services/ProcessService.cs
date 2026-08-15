// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The process service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller.Services;

/// <inheritdoc cref="IProcessService"/>
/// <seealso cref="IProcessService"/>
public class ProcessService : IProcessService
{
    /// <inheritdoc cref="IProcessService"/>
    /// <seealso cref="IProcessService"/>
    public int KillProcesses(Config config)
    {
        var killed = 0;
        var names = config.Processes
            .Select(p => p.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n));

        foreach (var name in names)
        {
            foreach (var runningProcess in System.Diagnostics.Process.GetProcessesByName(name))
            {
                using (runningProcess)
                {
                    try
                    {
                        runningProcess.Kill();
                        killed++;
                    }
                    catch (Exception ex)
                    {
                        // A process can exit on its own between the lookup and the kill, and a process of another
                        // user or a protected system process cannot be killed at all. Neither of them must stop
                        // the remaining processes from being killed.
                        Console.WriteLine($"The process {name} could not be killed: {ex.Message}");
                    }
                }
            }
        }

        return killed;
    }
}
