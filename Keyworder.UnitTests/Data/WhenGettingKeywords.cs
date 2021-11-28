using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Keyworder.UnitTests.Data;

[TestClass]
public class WhenGettingKeywords : TestBase
{
    [TestMethod]
    public async Task CanGetCategories()
    {
        // act
        var categories = await keywordService.GetCategoriesAsync();

        // assert
        categories.Count().Should().BeGreaterThan(0);
        foreach (var category in categories)
        {
            category.Should().NotBeNull();
            category.CategoryId.Should().NotBeNullOrWhiteSpace();
            category.Keywords.Should().NotBeNull();
            foreach (var keyword in category.Keywords)
            {
                keyword.Should().NotBeNull();
                keyword.CategoryId.Should().Be(category.CategoryId);
                keyword.KeywordId.Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}