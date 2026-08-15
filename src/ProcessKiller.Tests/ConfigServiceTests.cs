// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigServiceTests.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to test the <see cref="ConfigService" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessKiller.Tests;

/// <summary>
/// A class to test the <see cref="ConfigService"/> class.
/// </summary>
[TestClass]
public class ConfigServiceTests
{
    /// <summary>
    /// The configuration service under test.
    /// </summary>
    private readonly IConfigService configService = new ConfigService();

    /// <summary>
    /// Checks whether a configuration is deserialized at all. Up to version 1.0.7.0 the model class
    /// <see cref="Process"/> was abstract, which made this throw on every single start of the application.
    /// </summary>
    [TestMethod]
    public void ImportConfigurationFromStringReturnsTheConfiguredProcess()
    {
        var config = this.configService.ImportConfigurationFromString(TestDataProvider.GetConfigXml("Test"));

        Assert.AreEqual(1, config.Processes.Count);
        Assert.AreEqual("Test", config.Processes[0].Name);
        Assert.AreEqual("Test.exe", config.Processes[0].FullName);
    }

    /// <summary>
    /// Checks whether every entry of the list is returned and the order is kept.
    /// </summary>
    [TestMethod]
    public void ImportConfigurationFromStringReturnsAllConfiguredProcesses()
    {
        var config = this.configService.ImportConfigurationFromString(TestDataProvider.GetConfigXml("First", "Second", "Third"));

        Assert.AreEqual(3, config.Processes.Count);
        Assert.AreEqual("First", config.Processes[0].Name);
        Assert.AreEqual("Second", config.Processes[1].Name);
        Assert.AreEqual("Third", config.Processes[2].Name);
    }

    /// <summary>
    /// Checks whether an empty processes element results in an empty list instead of a null reference.
    /// </summary>
    [TestMethod]
    public void ImportConfigurationFromStringWithAnEmptyProcessesElementReturnsAnEmptyList()
    {
        var config = this.configService.ImportConfigurationFromString("<Config><Processes /></Config>");

        Assert.AreEqual(0, config.Processes.Count);
    }

    /// <summary>
    /// Checks whether broken XML is reported instead of being swallowed.
    /// </summary>
    [TestMethod]
    public void ImportConfigurationFromStringWithBrokenXmlThrowsAnInvalidOperationException()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => this.configService.ImportConfigurationFromString("<Config>"));
    }

    /// <summary>
    /// Checks whether the shipped configuration file can still be read. This is the file the installer puts next
    /// to the executable, so a change to it that breaks the deserialization breaks every installation.
    /// </summary>
    [TestMethod]
    public void ImportConfigurationReadsTheShippedConfigurationFile()
    {
        var config = this.configService.ImportConfiguration(TestDataProvider.ShippedConfigFileName);

        Assert.AreEqual(1, config.Processes.Count);
        Assert.AreEqual("Test", config.Processes[0].Name);
        Assert.AreEqual("Test.exe", config.Processes[0].FullName);
    }

    /// <summary>
    /// Checks whether a configuration written to disk is read back unchanged.
    /// </summary>
    [TestMethod]
    public void ImportConfigurationReadsTheFileFromDisk()
    {
        var directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(directory);

        try
        {
            var fileName = Path.Combine(directory, "Config.xml");
            File.WriteAllText(fileName, TestDataProvider.GetConfigXml("Notepad", "Calculator"));

            var config = this.configService.ImportConfiguration(fileName);

            Assert.AreEqual(2, config.Processes.Count);
            Assert.AreEqual("Notepad", config.Processes[0].Name);
            Assert.AreEqual("Calculator", config.Processes[1].Name);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    /// <summary>
    /// Checks whether a missing configuration file is reported with the file name instead of ending up in some
    /// deserialization error.
    /// </summary>
    [TestMethod]
    public void ImportConfigurationWithAMissingFileThrowsAFileNotFoundException()
    {
        var fileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "Config.xml");

        var exception = Assert.ThrowsExactly<FileNotFoundException>(() => this.configService.ImportConfiguration(fileName));

        Assert.AreEqual(fileName, exception.FileName);
    }
}
