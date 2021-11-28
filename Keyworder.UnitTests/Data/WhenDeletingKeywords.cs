using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Keyworder.UnitTests.Data;

[TestClass]
public class WhenDeletingKeywords : TestBase
{
    [TestMethod]
    public async Task CanDeleteExistingCategory()
    {
        // arrange
        var categories = await keywordService.GetCategoriesAsync();
        var testCategoryId = categories.First().CategoryId;

        // act
        await keywordService.DeleteCategoryAsync(testCategoryId);

        // assert
        categories = await keywordService.GetCategoriesAsync();
        categories.Should()
            .NotContain(c => c.CategoryId == testCategoryId);
    }

    [TestMethod]
    public async Task NoErrorWhenCategoryDoesntExist()
    {
        // arrange
        const string testCategoryId = "Foo";

        // act
        await keywordService.DeleteCategoryAsync(testCategoryId);

        // assert
        var categories = await keywordService.GetCategoriesAsync();
        categories.Should()
            .NotContain(c => c.CategoryId == testCategoryId);
    }

    [TestMethod]
    public async Task CanDeleteExistingKeyword()
    {
        //arrange
        var categories = await keywordService.GetCategoriesAsync();
        var testCategory = categories.First();
        var testCategoryId = testCategory.CategoryId;
        var testKeywordId = testCategory.Keywords.First().KeywordId;

        // act
        await keywordService.DeleteKeywordAsync(testCategoryId, testKeywordId);

        // assert
        categories = await keywordService.GetCategoriesAsync();
        categories.Single(c => c.CategoryId == testCategoryId)
            .Keywords.Should().NotContain(k => k.KeywordId == testKeywordId);
    }

    [TestMethod]
    public async Task NoErrorWhenKeywordDoesntExist()
    {
        //arrange
        var categories = await keywordService.GetCategoriesAsync();
        var testCategory = categories.First();
        var testCategoryId = testCategory.CategoryId;
        const string testKeywordId = "Foo";

        // act
        await keywordService.DeleteKeywordAsync(testCategoryId, testKeywordId);

        // assert
        categories = await keywordService.GetCategoriesAsync();
        categories.Single(c => c.CategoryId == testCategoryId)
            .Keywords.Should().NotContain(k => k.KeywordId == testKeywordId);
    }
}