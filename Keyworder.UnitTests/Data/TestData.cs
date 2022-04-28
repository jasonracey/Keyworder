using Keyworder.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Keyworder.UnitTests.Data;

public static class TestData
{
    public static void Create(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var keywords = new List<Keyword>
        {
            new()
            {
                Name = "Fruit",
                Children = new List<Keyword>
                {
                    new() { Name = "Apple" },
                    new() { Name = "Banana" },
                    new() { Name = "Cherry" }
                }
            },
            new()
            {
                Name = "Vegetables",
                Children = new List<Keyword>
                {
                    new() { Name = "Tomato" },
                    new() { Name = "Pepper" },
                    new() { Name = "Squash" }
                }
            }
        };

        var json = JsonConvert.SerializeObject(keywords);

        File.WriteAllText(path, json);
    }
}