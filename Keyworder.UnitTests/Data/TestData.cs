using Keyworder.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Keyworder.UnitTests.Data;

public static class TestData
{
    public static IEnumerable<Keyword> GetTestData()
    {
        return new List<Keyword>
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
    }
}