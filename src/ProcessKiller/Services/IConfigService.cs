// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The interface for the configuration service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller.Services;

/// <summary>
/// The interface for the configuration service.
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Imports the configuration from the given file.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    /// <returns>A new <see cref="Config"/> object.</returns>
    Config ImportConfiguration(string fileName);

    /// <summary>
    /// Imports the configuration from the given XML string.
    /// </summary>
    /// <param name="xml">The XML string.</param>
    /// <returns>A new <see cref="Config"/> object.</returns>
    Config ImportConfigurationFromString(string xml);
}
