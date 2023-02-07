using System;
using System.IO;
using System.Text;
using GraphManipulation.Helpers;
using J2N.Collections.Generic;
using Newtonsoft.Json;
using Xunit;

namespace Test.TestHelpers;

public class ConfigManagerTest
{
    [Fact]
    public void TestInitCreatesFile()
    {
        ConfigManager cf = new ConfigManager("testConfig.json");
        Assert.True(File.Exists("testConfig.json"));
        File.Delete("testConfig.json");
    }

    [Fact]
    public void testGetValueRaisesErrorOnEmptyValue()
    {
        ConfigManager cf = new ConfigManager("testConfig.json");

        Assert.Throws<Exception>(() => cf.getValue("OutputPath"));
        File.Delete("testConfig.json");
    }

    [Fact]
    public void TestGetValueReturnsCorrectValue()
    {
        string path = $"TestResources{Path.DirectorySeparatorChar}newConfigFile.json";
        
        Dictionary<string, string> configContent = new Dictionary<string, string>()
        {
            {"TestValue", "SomeValue"}
        };
        using (FileStream fs = File.Create(path))
        {
            char[] value = JsonConvert.SerializeObject(configContent).ToCharArray();
            fs.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
        }

        ConfigManager cf = new(path);
        Assert.True(cf.getValue("TestValue") == "SomeValue");

    }
}