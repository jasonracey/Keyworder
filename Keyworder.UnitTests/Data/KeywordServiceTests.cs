using FluentAssertions;
using Keyworder.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Keyworder.UnitTests.Data;

[TestClass]
public class KeywordServiceTests : TestBase
{
    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void Constructor_ValidatesArgs(string keywordsJsonPath)
    {
        // arrange
        Action act = () => new KeywordService(keywordsJsonPath);

        // act/assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("keywordsJsonPath");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task CreateCategory_ValidatesArgs(string categoryName)
    {
        // arrange
        var task = async () => { await keywordService.CreateCategoryAsync(categoryName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("categoryName");
    }

    [TestMethod]
    public async Task CreateCategory_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";

        // act
        var result = await keywordService.CreateCategoryAsync(categoryName);

        // assert
        result.Should().Be(ResultType.Created);
        await AssertCategoryExists(categoryNameClean);
        await AssertCategoryDoesNotExist(categoryName);
    }

    [TestMethod]
    public async Task CreateCategory_HtmlEncodesInput()
    {
        // arrange
        var categoryName = "<script>alert('hi');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);

        // act
        var result = await keywordService.CreateCategoryAsync(categoryName);

        // assert
        result.Should().Be(ResultType.Created);
        await AssertCategoryExists(categoryNameClean);
        await AssertCategoryDoesNotExist(categoryName);
    }

    [TestMethod]
    public async Task CreateCategory_PreventsDuplicateCategory()
    {
        // arrange
        var categoryName = Guid.NewGuid().ToString();
        var firstResult = await keywordService.CreateCategoryAsync(categoryName);

        // act
        var secondResult = await keywordService.CreateCategoryAsync(categoryName);

        // assert
        firstResult.Should().Be(ResultType.Created);
        secondResult.Should().Be(ResultType.Duplicate);
        await AssertCategoryExists(categoryName);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task CreateKeyword_ValidatesCategoryArg(string keywordName)
    {
        // arrange
        var task = async () => { await keywordService.CreateKeywordAsync("category", keywordName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("keywordName");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task CreateKeyword_ValidatesKeywordArg(string keywordName)
    {
        // arrange
        var task = async () => { await keywordService.CreateKeywordAsync("category", keywordName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("keywordName");
    }

    [TestMethod]
    public async Task CreateKeyword_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";
        var keywordNameClean = Guid.NewGuid().ToString();
        var keywordName = $" {keywordNameClean}  ";
        var categoryResult = await keywordService.CreateCategoryAsync(categoryName);

        // act
        var keywordResult = await keywordService.CreateKeywordAsync(categoryName, keywordName);

        // assert
        categoryResult.Should().Be(ResultType.Created);
        keywordResult.Should().Be(ResultType.Created);
        await AssertCategoryExists(categoryNameClean);
        await AssertCategoryDoesNotExist(categoryName);
        await AssertKeywordExists(categoryNameClean, keywordNameClean);
        await AssertKeywordDoesNotExist(categoryNameClean, keywordName);
    }

    [TestMethod]
    public async Task CreateKeyword_HtmlEncodesInput()
    {
        // arrange
        var categoryName = "<script>alert('hi');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);
        var keywordName = "<script>alert('world');</script>";
        var keywordNameClean = HttpUtility.HtmlEncode(keywordName);
        var categoryResult = await keywordService.CreateCategoryAsync(categoryName);

        // act
        var keywordResult = await keywordService.CreateKeywordAsync(categoryName, keywordName);

        // assert
        categoryResult.Should().Be(ResultType.Created);
        keywordResult.Should().Be(ResultType.Created);
        await AssertCategoryExists(categoryNameClean);
        await AssertCategoryDoesNotExist(categoryName);
        await AssertKeywordExists(categoryNameClean, keywordNameClean);
        await AssertKeywordDoesNotExist(categoryNameClean, keywordName);
    }

    [TestMethod]
    public async Task CreateKeyword_AllowsDuplicateKeywordInDifferentCategory()
    {
        // arrange
        var firstCategory = Guid.NewGuid().ToString();
        var secondCategory = Guid.NewGuid().ToString();
        var keyword = Guid.NewGuid().ToString();
        var firstCategoryResult = await keywordService.CreateCategoryAsync(firstCategory);
        var secondCategoryResult = await keywordService.CreateCategoryAsync(secondCategory);
        var firstKeywordResult = await keywordService.CreateKeywordAsync(firstCategory, keyword);

        // act
        var secondKeywordResult = await keywordService.CreateKeywordAsync(secondCategory, keyword);

        // assert
        firstCategoryResult.Should().Be(ResultType.Created);
        secondCategoryResult.Should().Be(ResultType.Created);
        firstKeywordResult.Should().Be(ResultType.Created);
        secondKeywordResult.Should().Be(ResultType.Created);
        await AssertCategoryExists(firstCategory);
        await AssertCategoryExists(secondCategory);
        await AssertKeywordExists(firstCategory, keyword);
        await AssertKeywordExists(secondCategory, keyword);
    }

    [TestMethod]
    public async Task CreateKeyword_PreventsDuplicateKeywordInSameCategory()
    {
        // arrange
        var category = Guid.NewGuid().ToString();
        var keyword = Guid.NewGuid().ToString();
        var categoryResult = await keywordService.CreateCategoryAsync(category);
        var firstKeywordResult = await keywordService.CreateKeywordAsync(category, keyword);

        // act
        var secondKeywordResult = await keywordService.CreateKeywordAsync(category, keyword);

        // assert
        categoryResult.Should().Be(ResultType.Created);
        firstKeywordResult.Should().Be(ResultType.Created);
        secondKeywordResult.Should().Be(ResultType.Duplicate);
        await AssertCategoryExists(category);
        await AssertKeywordExists(category, keyword);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task DeleteCategory_ValidatesArgs(string categoryName)
    {
        // arrange
        var task = async () => { await keywordService.DeleteCategoryAsync(categoryName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("categoryName");
    }

    [TestMethod]
    public async Task DeleteCategory_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";
        var createResult = await keywordService.CreateCategoryAsync(categoryName);

        // act
        var deleteResult = await keywordService.DeleteCategoryAsync(categoryName);

        // assert
        createResult.Should().Be(ResultType.Created);
        deleteResult.Should().Be(ResultType.Deleted);
        await AssertCategoryDoesNotExist(categoryName);
        await AssertCategoryDoesNotExist(categoryNameClean);
    }

    [TestMethod]
    public async Task DeleteCategory_HtmlEncodesInput()
    {
        // arrange
        var categoryName = "<script>alert('hi');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);
        var createResult = await keywordService.CreateCategoryAsync(categoryName);

        // act
        var deleteResult = await keywordService.DeleteCategoryAsync(categoryName);

        // assert
        createResult.Should().Be(ResultType.Created);
        deleteResult.Should().Be(ResultType.Deleted);
        await AssertCategoryDoesNotExist(categoryName);
        await AssertCategoryDoesNotExist(categoryNameClean);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task DeleteKeyword_ValidatesCategoryArg(string categoryName)
    {
        // arrange
        var task = async () => { await keywordService.DeleteKeywordAsync(categoryName, "keyword"); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("categoryName");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task DeleteKeyword_ValidatesKeywordArg(string keywordName)
    {
        // arrange
        var task = async () => { await keywordService.DeleteKeywordAsync("category", keywordName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("keywordName");
    }

    [TestMethod]
    public async Task DeleteKeyword_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";
        var keywordNameClean = Guid.NewGuid().ToString();
        var keywordName = $" {keywordNameClean}  ";
        var createCategoryResult = await keywordService.CreateCategoryAsync(categoryName);
        var createKeywordResult = await keywordService.CreateKeywordAsync(categoryName, keywordName);

        // act
        var deleteResult = await keywordService.DeleteKeywordAsync(categoryName, keywordName);

        // assert
        createCategoryResult.Should().Be(ResultType.Created);
        createKeywordResult.Should().Be(ResultType.Created);
        deleteResult.Should().Be(ResultType.Deleted);
        await AssertCategoryExists(categoryNameClean);
        await AssertCategoryDoesNotExist(categoryName);
        await AssertKeywordDoesNotExist(categoryNameClean, keywordName);
        await AssertKeywordDoesNotExist(categoryNameClean, keywordNameClean);
    }

    [TestMethod]
    public async Task DeleteKeyword_HtmlEncodesInput()
    {
        // arrange
        var categoryName = "<script>alert('hi');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);
        var keywordName = "<script>alert('world');</script>";
        var keywordNameClean = HttpUtility.HtmlEncode(keywordName);
        var createCategoryResult = await keywordService.CreateCategoryAsync(categoryName);
        var createKeywordResult = await keywordService.CreateKeywordAsync(categoryName, keywordName);

        // act
        var deleteResult = await keywordService.DeleteKeywordAsync(categoryName, keywordName);

        // assert
        createCategoryResult.Should().Be(ResultType.Created);
        createKeywordResult.Should().Be(ResultType.Created);
        deleteResult.Should().Be(ResultType.Deleted);
        await AssertCategoryExists(categoryNameClean);
        await AssertCategoryDoesNotExist(categoryName);
        await AssertKeywordDoesNotExist(categoryNameClean, keywordName);
        await AssertKeywordDoesNotExist(categoryNameClean, keywordNameClean);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditCategory_ValidatesOldCategoryArg(string oldCategoryName)
    {
        // arrange
        var task = async () => { await keywordService.EditCategoryAsync(oldCategoryName, "newCategory"); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("oldCategoryName");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditCategory_ValidatesNewCategoryArg(string newCategoryName)
    {
        // arrange
        var task = async () => { await keywordService.EditCategoryAsync("oldCategory", newCategoryName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("newCategoryName");
    }

    [TestMethod]
    public async Task EditCategory_TrimsInput()
    {
        // arrange
        var oldCategoryNameClean = Guid.NewGuid().ToString();
        var oldCategoryName = $" {oldCategoryNameClean}  ";
        var newCategoryNameClean = Guid.NewGuid().ToString();
        var newCategoryName = $" {newCategoryNameClean}  ";
        var createResult = await keywordService.CreateCategoryAsync(oldCategoryName);

        // act
        var editResult = await keywordService.EditCategoryAsync(oldCategoryName, newCategoryName);

        // assert
        createResult.Should().Be(ResultType.Created);
        editResult.Should().Be(ResultType.Edited);
        await AssertCategoryDoesNotExist(oldCategoryNameClean);
        await AssertCategoryDoesNotExist(oldCategoryName);
        await AssertCategoryDoesNotExist(newCategoryName);
        await AssertCategoryExists(newCategoryNameClean);
    }

    [TestMethod]
    public async Task EditCategory_HtmlEncodesInput()
    {
        // arrange
        var oldCategoryName = "<script>alert('hi');</script>";
        var oldCategoryNameClean = HttpUtility.HtmlEncode(oldCategoryName);
        var newCategoryName = "<script>alert('world');</script>";
        var newCategoryNameClean = HttpUtility.HtmlEncode(newCategoryName);
        var createResult = await keywordService.CreateCategoryAsync(oldCategoryName);

        // act
        var editResult = await keywordService.EditCategoryAsync(oldCategoryName, newCategoryName);

        // assert
        createResult.Should().Be(ResultType.Created);
        editResult.Should().Be(ResultType.Edited);
        await AssertCategoryDoesNotExist(oldCategoryNameClean);
        await AssertCategoryDoesNotExist(oldCategoryName);
        await AssertCategoryDoesNotExist(newCategoryName);
        await AssertCategoryExists(newCategoryNameClean);
    }

    [TestMethod]
    public async Task EditCategory_PreventsDuplicateCategory()
    {
        // arrange
        var oldCategoryName = Guid.NewGuid().ToString();
        var newCategoryName = Guid.NewGuid().ToString();
        var oldCreateResult = await keywordService.CreateCategoryAsync(oldCategoryName);
        var newCreateResult = await keywordService.CreateCategoryAsync(newCategoryName);

        // act
        var editResult = await keywordService.EditCategoryAsync(oldCategoryName, newCategoryName);

        // assert
        oldCreateResult.Should().Be(ResultType.Created);
        newCreateResult.Should().Be(ResultType.Created);
        editResult.Should().Be(ResultType.Duplicate);
        await AssertCategoryExists(oldCategoryName);
        await AssertCategoryExists(newCategoryName);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditKeyword_ValidatesCategoryArg(string categoryName)
    {
        // arrange
        var task = async () => { await keywordService.EditKeywordAsync(categoryName, "oldKeyword", "newKeyword"); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("categoryName");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditKeyword_ValidatesOldKeywordArg(string oldKeywordName)
    {
        // arrange
        var task = async () => { await keywordService.EditKeywordAsync("category", oldKeywordName, "newKeyword"); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("oldKeywordName");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditCategory_ValidatesNewKeywordArg(string newKeywordName)
    {
        // arrange
        var task = async () => { await keywordService.EditKeywordAsync("category", "oldKeyword", newKeywordName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("newKeywordName");
    }

    [TestMethod]
    public async Task EditKeyword_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";
        var oldKeywordNameClean = Guid.NewGuid().ToString();
        var oldKeywordName = $" {oldKeywordNameClean}  ";
        var newKeywordNameClean = Guid.NewGuid().ToString();
        var newKeywordName = $" {newKeywordNameClean}  ";
        var createCategoryResult = await keywordService.CreateCategoryAsync(categoryName);
        var createOldKeywordResult = await keywordService.CreateKeywordAsync(categoryName, oldKeywordName);

        // act
        var editResult = await keywordService.EditKeywordAsync(categoryName, oldKeywordName, newKeywordName);

        // assert
        createCategoryResult.Should().Be(ResultType.Created);
        createOldKeywordResult.Should().Be(ResultType.Created);
        editResult.Should().Be(ResultType.Edited);
        await AssertCategoryDoesNotExist(categoryName);
        await AssertCategoryExists(categoryNameClean);
        await AssertKeywordDoesNotExist(categoryNameClean, oldKeywordName);
        await AssertKeywordDoesNotExist(categoryNameClean, oldKeywordNameClean);
        await AssertKeywordDoesNotExist(categoryNameClean, newKeywordName);
        await AssertKeywordExists(categoryNameClean, newKeywordNameClean);
    }

    [TestMethod]
    public async Task EditKeyword_HtmlEncodesInput()
    {
        // arrange
        var categoryName = "<script>alert('foo');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);
        var oldKeywordName = "<script>alert('bar');</script>";
        var oldKeywordNameClean = HttpUtility.HtmlEncode(oldKeywordName);
        var newKeywordName = "<script>alert('baz');</script>";
        var newKeywordNameClean = HttpUtility.HtmlEncode(newKeywordName);
        var createCategoryResult = await keywordService.CreateCategoryAsync(categoryName);
        var createOldKeywordResult = await keywordService.CreateKeywordAsync(categoryName, oldKeywordName);

        // act
        var editResult = await keywordService.EditKeywordAsync(categoryName, oldKeywordName, newKeywordName);

        // assert
        createCategoryResult.Should().Be(ResultType.Created);
        createOldKeywordResult.Should().Be(ResultType.Created);
        editResult.Should().Be(ResultType.Edited);
        await AssertCategoryDoesNotExist(categoryName);
        await AssertCategoryExists(categoryNameClean);
        await AssertKeywordDoesNotExist(categoryNameClean, oldKeywordName);
        await AssertKeywordDoesNotExist(categoryNameClean, oldKeywordNameClean);
        await AssertKeywordDoesNotExist(categoryNameClean, newKeywordName);
        await AssertKeywordExists(categoryNameClean, newKeywordNameClean);
    }

    [TestMethod]
    public async Task EditKeyword_AllowsDuplicateKeywordInDifferentCategory()
    {
        // arrange
        var firstCategory = Guid.NewGuid().ToString();
        var secondCategory = Guid.NewGuid().ToString();
        var firstKeyword = Guid.NewGuid().ToString();
        var secondKeyword = Guid.NewGuid().ToString();
        var firstCategoryResult = await keywordService.CreateCategoryAsync(firstCategory);
        var secondCategoryResult = await keywordService.CreateCategoryAsync(secondCategory);
        var firstKeywordResult = await keywordService.CreateKeywordAsync(firstCategory, firstKeyword);
        var secondKeywordResult = await keywordService.CreateKeywordAsync(secondCategory, secondKeyword);

        // act
        var editResult = await keywordService.EditKeywordAsync(firstCategory, firstKeyword, secondKeyword);

        // assert
        firstCategoryResult.Should().Be(ResultType.Created);
        secondCategoryResult.Should().Be(ResultType.Created);
        firstKeywordResult.Should().Be(ResultType.Created);
        secondKeywordResult.Should().Be(ResultType.Created);
        editResult.Should().Be(ResultType.Edited);
        await AssertCategoryExists(firstCategory);
        await AssertCategoryExists(secondCategory);
        await AssertKeywordDoesNotExist(firstCategory, firstKeyword);
        await AssertKeywordExists(firstCategory, secondKeyword);
        await AssertKeywordExists(secondCategory, secondKeyword);
    }

    [TestMethod]
    public async Task EditKeyword_PreventsDuplicateKeywordInSameCategory()
    {
        // arrange
        var category = Guid.NewGuid().ToString();
        var firstKeyword = Guid.NewGuid().ToString();
        var secondKeyword = Guid.NewGuid().ToString();
        var categoryResult = await keywordService.CreateCategoryAsync(category);
        var firstKeywordResult = await keywordService.CreateKeywordAsync(category, firstKeyword);
        var secondKeywordResult = await keywordService.CreateKeywordAsync(category, secondKeyword);

        // act
        var editResult = await keywordService.EditKeywordAsync(category, firstKeyword, secondKeyword);

        // assert
        categoryResult.Should().Be(ResultType.Created);
        firstKeywordResult.Should().Be(ResultType.Created);
        secondKeywordResult.Should().Be(ResultType.Created);
        editResult.Should().Be(ResultType.Duplicate);
        await AssertCategoryExists(category);
        await AssertKeywordExists(category, firstKeyword);
        await AssertKeywordExists(category, secondKeyword);
    }

    [TestMethod]
    public async Task CanGetKeywords()
    {
        // act
        var keywords = await keywordService.GetKeywordsAsync();

        // assert
        keywords.Count().Should().BeGreaterThan(0);
        foreach (var keyword in keywords)
        {
            Assert.IsNotNull(keyword);
            keyword.Should().NotBeNull();
            keyword.Name.Should().NotBeNullOrWhiteSpace();
            keyword.Children.Should().NotBeNull();
            keyword.Children.Count().Should().BeGreaterThan(0);
            foreach (var child in keyword.Children)
            {
                child.Should().NotBeNull();
                child.Name.Should().NotBeNullOrWhiteSpace();
                child.Children.Should().NotBeNull();
                child.Children.Should().BeEmpty();
            }
        }
    }

    private async Task AssertCategoryExists(string categoryName)
    {
        var keywords = await keywordService.GetKeywordsAsync();
        keywords.Should().Contain(item =>
            item.IsCategory &&
            item.Name.Equals(categoryName, StringComparison.Ordinal));
    }

    private async Task AssertCategoryDoesNotExist(string categoryName)
    {
        var keywords = await keywordService.GetKeywordsAsync();
        keywords.Should().NotContain(item =>
            item.Name.Equals(categoryName, StringComparison.Ordinal));
    }

    private async Task AssertKeywordExists(string categoryName, string keywordName)
    {
        var keywords = await keywordService.GetKeywordsAsync();
        keywords.Should().Contain(item =>
            item.IsCategory &&
            item.Name.Equals(categoryName) &&
            item.Children.Count(keyword => 
                !keyword.IsCategory &&
                keyword.Name.Equals(keywordName, StringComparison.Ordinal) &&
                !keyword.Children.Any()) == 1);
    }

    private async Task AssertKeywordDoesNotExist(string categoryName, string keywordName)
    {
        var keywords = await keywordService.GetKeywordsAsync();
        keywords.Should().Contain(item =>
            item.IsCategory &&
            item.Name.Equals(categoryName) &&
            !item.Children.Any(keyword =>
                keyword.Name.Equals(keywordName, StringComparison.Ordinal)));
    }
}