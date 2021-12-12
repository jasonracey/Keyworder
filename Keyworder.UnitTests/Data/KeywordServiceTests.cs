using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Keyworder.UnitTests.Data;

[TestClass]
public class KeywordServiceTests : TestBase
{
    [TestMethod]
    public async Task CanGetKeywords()
    {
        // act
        var keywords = await keywordService.GetKeywordsAsync();

        // assert
        keywords.Count().Should().BeGreaterThan(0);
        foreach (var keyword in keywords)
        {
            keyword.Should().NotBeNull();
            keyword.Name.Should().NotBeNullOrWhiteSpace();
            keyword.Children.Should().NotBeNull();
            keyword.Children.Count().Should().BeGreaterThan(0);
            foreach (var child in keyword.Children)
            {
                child.Should().NotBeNull();
                child.Name.Should().NotBeNullOrWhiteSpace();
                child.Children.Should().BeNull();
            }
        }
    }
}