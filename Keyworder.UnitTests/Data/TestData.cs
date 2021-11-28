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

        const string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Categories>
    <Category CategoryId=""Animal-Common"">
        <Keyword KeywordId=""Animal""/>
        <Keyword KeywordId=""Bird""/>
        <Keyword KeywordId=""Black-Tailed Deer""/>
    </Category>
    <Category CategoryId=""Animal-Latin"">
        <Keyword KeywordId=""Lagopus Leucura""/>
        <Keyword KeywordId=""Marmota Caligata""/>
        <Keyword KeywordId=""Odocoileus Hemionus""/>
    </Category>
    <Category CategoryId=""Color"">
        <Keyword KeywordId=""Black""/>
        <Keyword KeywordId=""Blue""/>
        <Keyword KeywordId=""Indigo""/>
    </Category>
</Categories>";

        File.WriteAllText(path, xml);
    }
}