// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The configuration service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller.Services;

/// <inheritdoc cref="IConfigService"/>
/// <seealso cref="IConfigService"/>
public class ConfigService : IConfigService
{
    /// <inheritdoc cref="IConfigService"/>
    /// <seealso cref="IConfigService"/>
    public Config ImportConfiguration(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException("The configuration file was not found.", fileName);
        }

        var xDocument = XDocument.Load(fileName);
        return this.ImportConfigurationFromString(xDocument.ToString());
    }

    /// <inheritdoc cref="IConfigService"/>
    /// <seealso cref="IConfigService"/>
    public Config ImportConfigurationFromString(string xml)
    {
        var xmlSerializer = new XmlSerializer(typeof(Config));
        using var stringReader = new StringReader(xml);
        return xmlSerializer.Deserialize(stringReader) as Config ?? new Config();
    }
}
