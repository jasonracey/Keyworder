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
            new Keyword
            {
                Name = "Fruit",
                Keywords = new List<Keyword>
                {
                    new Keyword { Name = "Apple" },
                    new Keyword { Name = "Banana" },
                    new Keyword { Name = "Cherry" }
                }
            },
            new Keyword
            {
                Name = "Vegetables",
                Keywords = new List<Keyword>
                {
                    new Keyword { Name = "Tomato" },
                    new Keyword { Name = "Pepper" },
                    new Keyword { Name = "Squash" }
                }
            }
        };

        var json = JsonConvert.SerializeObject(keywords);

        File.WriteAllText(path, json);
    }
}