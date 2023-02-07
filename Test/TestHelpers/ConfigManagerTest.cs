using System.Collections.Generic;
using System.IO;
using System.Text;
using GraphManipulation.Helpers;
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
    public void GetValueReturnsEmptyValue()
    {
        ConfigManager cf = new ConfigManager("testConfig.json");

        Assert.True(cf.GetValue("OutputPath") == "");
        File.Delete("testConfig.json");
    }

    [Fact]
    public void GetValueRaisesKeyNotFoundOnIncorrectKey()
    {
        ConfigManager cf = new ConfigManager("testConfig.json");

        Assert.Throws<KeyNotFoundException>(() => cf.GetValue("ThisKeyDoesNotExist"));
        File.Delete("testConfig.json");
    }


    [Fact]
    public void TestGetValueReturnsCorrectValue()
    {
        string path = $"TestResources{Path.DirectorySeparatorChar}newConfigFile.json";

        Dictionary<string, string> configContent =
            new Dictionary<string, string>()
            {
                {"TestValue", "SomeValue"}
            };
        using (FileStream fs = File.Create(path))
        {
            char[] value = JsonConvert.SerializeObject(configContent).ToCharArray();
            fs.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
        }

        ConfigManager cf = new(path);
        Assert.True(cf.GetValue("TestValue") == "SomeValue");
    }
}