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
[Serializable]
public class Process
{
    /// <summary>
    /// Gets or sets the name. This is the name without the file extension, the way
    /// <see cref="System.Diagnostics.Process.GetProcessesByName(string)"/> expects it.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name, meaning the file name including its extension.
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}
