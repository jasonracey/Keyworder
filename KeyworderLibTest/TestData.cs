using System;
using System.IO;

namespace KeyworderLibTest
{
    public static class TestData
    {
        public static string GetTestData()
        {
            return File.ReadAllText("Keywords.xml");
        }

        public static string GetTestPath()
        {
            return $"{Guid.NewGuid()}.xml";
        }
    }
}
