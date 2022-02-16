// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Process.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The process model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller;

/// <summary>
/// The process model class.
/// </summary>
public abstract class Process
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}
