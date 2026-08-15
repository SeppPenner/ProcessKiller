// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataProvider.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to provide the test data used in the tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller.Tests;

/// <summary>
/// A class to provide the test data used in the tests.
/// </summary>
public static class TestDataProvider
{
    /// <summary>
    /// The file name of the shipped configuration file as it is copied to the output directory.
    /// </summary>
    public const string ShippedConfigFileName = "TestData\\Config.xml";

    /// <summary>
    /// Gets a configuration XML string that carries the given process names.
    /// </summary>
    /// <param name="names">The process names.</param>
    /// <returns>A configuration XML <see cref="string"/>.</returns>
    public static string GetConfigXml(params string[] names)
    {
        var processes = names.Select(n => $"\t\t<Process>\r\n\t\t\t<Name>{n}</Name>\r\n\t\t\t<FullName>{n}.exe</FullName>\r\n\t\t</Process>");
        return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"
            + "<Config xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
            + "\t<Processes>\r\n"
            + string.Join("\r\n", processes)
            + "\r\n\t</Processes>\r\n"
            + "</Config>";
    }

    /// <summary>
    /// Gets a <see cref="Config"/> that carries the given process names.
    /// </summary>
    /// <param name="names">The process names.</param>
    /// <returns>A new <see cref="Config"/> object.</returns>
    public static Config GetConfig(params string[] names)
    {
        return new Config
        {
            Processes = names.Select(n => new Process { Name = n, FullName = $"{n}.exe" }).ToList()
        };
    }
}
