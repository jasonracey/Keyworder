using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Keyworder.UnitTests.Data;

[TestClass]
public class WhenCreatingKeywords : TestBase
{
    [TestMethod]
    public async Task CanCreateNewCategory()
    {
        // arrange
        const string testCategoryId = "TestCategory";
        await keywordService.CreateCategoryAsync(testCategoryId);

        // act
        var categories = await keywordService.GetCategoriesAsync();
        
        // assert
        categories.Should()
            .Contain(c => c.CategoryId == testCategoryId);
    }

    [TestMethod]
    public async Task CannotCreateDuplicateCategory()
    {
        // arrange
        var categories = await keywordService.GetCategoriesAsync();
        var categoryWithAtLeastOneKeyword = categories.First(c => c.Keywords.Count > 0);
        var keywordCount = categoryWithAtLeastOneKeyword.Keywords.Count;
        var testCategoryId = categoryWithAtLeastOneKeyword.CategoryId;

        // act
        await keywordService.CreateCategoryAsync(testCategoryId);

        // assert
        categories = await keywordService.GetCategoriesAsync();
        var unchangedCategory = categories.Single(c => c.CategoryId == testCategoryId);
        unchangedCategory.Keywords.Count.Should().Be(keywordCount);

        // also check file because sorted set only contains distinct items
        XDocument.Load(path)
            .Descendants("Category")
            .Count(e => e?.Attribute("CategoryId")?.Value == testCategoryId)
            .Should().Be(1);
    }

    [TestMethod]
    public async Task CanCreateNewKeyword()
    {
        // arrange
        var categories = await keywordService.GetCategoriesAsync();
        var testCategoryId = categories.First().CategoryId;
        const string testKeywordId = "TestKeyword";

        // act
        await keywordService.CreateKeywordAsync(testCategoryId, testKeywordId);

        // assert
        categories = await keywordService.GetCategoriesAsync();
        categories.Single(c => c.CategoryId == testCategoryId)
            .Keywords.Should().Contain(k => k.KeywordId == testKeywordId);
    }

    [TestMethod]
    public async Task CanCreateDuplicateKeywordInDifferentCategory()
    {
        // arrange
        var categories = await keywordService.GetCategoriesAsync();
        var existingKeywordId = categories.First(c => c.Keywords.Count > 0)
            .Keywords.First().KeywordId;
        var differentCategoryId = categories.First(c => c.Keywords
            .All(k => k.KeywordId != existingKeywordId)).CategoryId;

        // act
        await keywordService.CreateKeywordAsync(differentCategoryId, existingKeywordId);

        // assert
        categories = await keywordService.GetCategoriesAsync();
        categories.Single(c => c.CategoryId == differentCategoryId)
            .Keywords.Should().Contain(k => k.KeywordId == existingKeywordId);
    }

    [TestMethod]
    public async Task CannotCreateDuplicateKeywordWithinCategory()
    {
        // arrange
        var categories = await keywordService.GetCategoriesAsync();
        var testCategory = categories.First(c => c.Keywords.Count > 0);
        var testCategoryId = testCategory.CategoryId;
        var existingKeywordId = testCategory.Keywords.First().KeywordId;

        // act
        await keywordService.CreateKeywordAsync(testCategoryId, existingKeywordId);

        // assert
        XDocument.Load(path)
            .Descendants("Category").Single(e => e?.Attribute("CategoryId")?.Value == testCategoryId)
            .Descendants("Keyword").Count(e => e?.Attribute("KeywordId")?.Value == existingKeywordId)
            .Should().Be(1);
    }
}