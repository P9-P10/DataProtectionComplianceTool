﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using GraphManipulation.Managers;
using Newtonsoft.Json;
using Xunit;

namespace Test;

public class ConfigManagerTest
{
    [Fact]
    public void TestInitCreatesFile()
    {
        var cf = new ConfigManager("testConfig.json");
        Assert.True(File.Exists("testConfig.json"));
        File.Delete("testConfig.json");
    }

    [Fact]
    public void GetValueReturnsEmptyValue()
    {
        var cf = new ConfigManager("testConfig.json");

        Assert.True(cf.GetValue("DatabaseConnectionString") == "");
        File.Delete("testConfig.json");
    }

    [Fact]
    public void GetValueRaisesKeyNotFoundOnIncorrectKey()
    {
        var cf = new ConfigManager("testConfig.json");

        Assert.Throws<KeyNotFoundException>(() => cf.GetValue("ThisKeyDoesNotExist"));
        File.Delete("testConfig.json");
    }


    [Fact]
    public void TestGetValueReturnsCorrectValue()
    {
        var path = $"TestResources{Path.DirectorySeparatorChar}newConfigFile.json";

        var configContent =
            new Dictionary<string, string>
            {
                {"TestValue", "SomeValue"}
            };
        using (var fs = File.Create(path))
        {
            var value = JsonConvert.SerializeObject(configContent).ToCharArray();
            fs.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
        }

        ConfigManager cf = new(path);
        Assert.True(cf.GetValue("TestValue") == "SomeValue");
    }

    [Fact]
    public void TestGetEmptyKeysReturnsAllEmptyValues()
    {
        var path = $"TestResources{Path.DirectorySeparatorChar}newConfigFile.json";

        var configContent =
            new Dictionary<string, string>
            {
                {"TestValue", ""}
            };
        using (var fs = File.Create(path))
        {
            var value = JsonConvert.SerializeObject(configContent).ToCharArray();
            fs.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
        }

        ConfigManager cf = new(path);
        List<string> emptyKeys = cf.GetEmptyKeys();
        Assert.True(emptyKeys.Count == 1);
        Assert.Contains("TestValue", emptyKeys);
    }
}