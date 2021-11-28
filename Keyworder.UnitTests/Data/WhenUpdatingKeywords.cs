using FluentAssertions;
using Keyworder.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Keyworder.UnitTests.Data;

[TestClass]
public class WhenUpdatingKeywords : TestBase
{
    [TestMethod]
    public async Task CanUpdateCategory()
    {
        // arrange
        const string newCategoryId = "TestCategory";
        var oldCategories = await keywordService.GetCategoriesAsync();
        oldCategories.Should().NotContain(c => c.CategoryId == newCategoryId);
        var oldCategoryId = oldCategories.First().CategoryId;
        oldCategoryId.Should().NotBe(newCategoryId);

        // act
        await keywordService.UpdateCategoryAsync(oldCategoryId, newCategoryId);

        // assert
        var newCategories = await keywordService.GetCategoriesAsync();
        newCategories.Should().NotContain(c => c.CategoryId == oldCategoryId);
        newCategories.Count(c => c.CategoryId == newCategoryId).Should().Be(1);
    }

    [TestMethod]
    public async Task ExceptionThrownWhenCategoryDoesntExist()
    {
        // arrange
        const string oldCategoryId = "Foo";
        const string newCategoryId = "Bar";
        var categories = await keywordService.GetCategoriesAsync();
        categories.Count(c => c.CategoryId == oldCategoryId).Should().Be(0);

        // act/assert
        await Assert.ThrowsExceptionAsync<KeyworderException>(() => keywordService.UpdateCategoryAsync(oldCategoryId, newCategoryId));
    }

    [TestMethod]
    public async Task CanUpdateKeyword()
    {
        // arrange
        const string newKeywordId = "TestKeyword";
        var categories = await keywordService.GetCategoriesAsync();
        var category = categories.First();
        var categoryId = category.CategoryId;
        var oldKeywordId = category.Keywords.First().KeywordId;
        category.Keywords.Count(k => k.KeywordId == oldKeywordId).Should().Be(1);
        category.Keywords.Should().NotContain(c => c.KeywordId == newKeywordId);
        oldKeywordId.Should().NotBe(newKeywordId);

        // act
        await keywordService.UpdateKeywordAsync(categoryId, oldKeywordId, newKeywordId);

        // assert
        var newCategories = await keywordService.GetCategoriesAsync();
        newCategories.Single(c => c.CategoryId == categoryId)
            .Keywords.Count(k => k.KeywordId == newKeywordId)
            .Should().Be(1);
        newCategories.Single(c => c.CategoryId == categoryId)
            .Keywords.Should()
            .NotContain(k => k.KeywordId == oldKeywordId);
    }

    [TestMethod]
    public async Task ExceptionThrownWhenKeywordDoesntExist()
    {
        // arrange
        const string oldKeywordId = "Foo";
        const string newKeywordId = "Bar";
        var categories = await keywordService.GetCategoriesAsync();
        var category = categories.First();
        category.Keywords.Count(k => k.KeywordId == oldKeywordId).Should().Be(0);

        // act/assert
        await Assert.ThrowsExceptionAsync<KeyworderException>(() => keywordService.UpdateKeywordAsync(category.CategoryId, oldKeywordId, newKeywordId));
    }
}