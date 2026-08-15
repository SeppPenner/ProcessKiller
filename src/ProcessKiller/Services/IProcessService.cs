// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The interface for the process service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller.Services;

/// <summary>
/// The interface for the process service.
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// Kills every running process that carries one of the names of the configuration.
    /// A process that cannot be killed is reported on the console and does not stop the run.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>The number of processes the kill was requested for.</returns>
    int KillProcesses(Config config);
}
