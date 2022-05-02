using FluentAssertions;
using Keyworder.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Moq;

namespace Keyworder.UnitTests.Data;

[TestClass]
public class KeywordServiceTests
{
    private Mock<IKeywordRepository> _mockKeywordRepository = new();
    
    private KeywordService _keywordService = new(Mock.Of<IKeywordRepository>());

    [TestInitialize]
    public void TestInitialize()
    {
        _mockKeywordRepository = new Mock<IKeywordRepository>();
        
        _mockKeywordRepository
            .Setup(mock => mock.ReadAsync())
            .ReturnsAsync(TestData.GetTestData());
        
        _mockKeywordRepository
            .Setup(mock => mock.WriteAsync(It.IsAny<IEnumerable<Keyword>>()))
            .ReturnsAsync((IEnumerable<Keyword> keywords) => keywords);

        _keywordService = new KeywordService(_mockKeywordRepository.Object);
    }
    
    [TestMethod]
    public void Constructor_ValidatesArgs()
    {
        // arrange
        var action = () =>
        {
            var _ = new KeywordService(null!);
        };

        // act/assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("keywordRepository");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task CreateCategory_ValidatesArgs(string categoryName)
    {
        // arrange
        var task = async () => { await _keywordService.CreateCategoryAsync(categoryName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(categoryName));
    }

    [TestMethod]
    public async Task CreateCategory_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";

        // act
        var keywordResult = await _keywordService.CreateCategoryAsync(categoryName);

        // assert
        keywordResult.ResultType.Should().Be(ResultType.Created);
        AssertCategoryExists(keywordResult.Keywords, categoryNameClean);
        AssertCategoryDoesNotExist(keywordResult.Keywords, categoryName);
    }

    [TestMethod]
    public async Task CreateCategory_HtmlEncodesInput()
    {
        // arrange
        const string categoryName = "<script>alert('hi');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);

        // act
        var keywordResult = await _keywordService.CreateCategoryAsync(categoryName);

        // assert
        keywordResult.ResultType.Should().Be(ResultType.Created);
        AssertCategoryExists(keywordResult.Keywords, categoryNameClean);
        AssertCategoryDoesNotExist(keywordResult.Keywords, categoryName);
    }

    [TestMethod]
    public async Task CreateCategory_PreventsDuplicateCategory()
    {
        // arrange
        var categoryName = Guid.NewGuid().ToString();
        var firstResult = await _keywordService.CreateCategoryAsync(categoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(firstResult.Keywords);

        // act
        var secondResult = await _keywordService.CreateCategoryAsync(categoryName);

        // assert
        firstResult.ResultType.Should().Be(ResultType.Created);
        secondResult.ResultType.Should().Be(ResultType.Duplicate);
        AssertCategoryExists(secondResult.Keywords, categoryName);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task CreateKeyword_ValidatesCategoryArg(string keywordName)
    {
        // arrange
        var task = async () => { await _keywordService.CreateKeywordAsync("category", keywordName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(keywordName));
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task CreateKeyword_ValidatesKeywordArg(string keywordName)
    {
        // arrange
        var task = async () => { await _keywordService.CreateKeywordAsync("category", keywordName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(keywordName));
    }

    [TestMethod]
    public async Task CreateKeyword_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";
        var keywordNameClean = Guid.NewGuid().ToString();
        var keywordName = $" {keywordNameClean}  ";
        var categoryResult = await _keywordService.CreateCategoryAsync(categoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(categoryResult.Keywords);

        // act
        var keywordResult = await _keywordService.CreateKeywordAsync(categoryName, keywordName);

        // assert
        categoryResult.ResultType.Should().Be(ResultType.Created);
        keywordResult.ResultType.Should().Be(ResultType.Created);
        AssertCategoryExists(keywordResult.Keywords, categoryNameClean);
        AssertCategoryDoesNotExist(keywordResult.Keywords, categoryName);
        AssertKeywordExists(keywordResult.Keywords, categoryNameClean, keywordNameClean);
        AssertKeywordDoesNotExist(keywordResult.Keywords, categoryNameClean, keywordName);
    }

    [TestMethod]
    public async Task CreateKeyword_HtmlEncodesInput()
    {
        // arrange
        const string categoryName = "<script>alert('hi');</script>";
        const string keywordName = "<script>alert('world');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);
        var keywordNameClean = HttpUtility.HtmlEncode(keywordName);
        var categoryResult = await _keywordService.CreateCategoryAsync(categoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(categoryResult.Keywords);

        // act
        var keywordResult = await _keywordService.CreateKeywordAsync(categoryName, keywordName);

        // assert
        categoryResult.ResultType.Should().Be(ResultType.Created);
        keywordResult.ResultType.Should().Be(ResultType.Created);
        AssertCategoryExists(keywordResult.Keywords, categoryNameClean);
        AssertCategoryDoesNotExist(keywordResult.Keywords, categoryName);
        AssertKeywordExists(keywordResult.Keywords, categoryNameClean, keywordNameClean);
        AssertKeywordDoesNotExist(keywordResult.Keywords, categoryNameClean, keywordName);
    }

    [TestMethod]
    public async Task CreateKeyword_AllowsDuplicateKeywordInDifferentCategory()
    {
        // arrange
        var firstCategory = Guid.NewGuid().ToString();
        var secondCategory = Guid.NewGuid().ToString();
        var keyword = Guid.NewGuid().ToString();
        var firstCategoryResult = await _keywordService.CreateCategoryAsync(firstCategory);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(firstCategoryResult.Keywords);
        var secondCategoryResult = await _keywordService.CreateCategoryAsync(secondCategory);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(secondCategoryResult.Keywords);
        var firstKeywordResult = await _keywordService.CreateKeywordAsync(firstCategory, keyword);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(firstKeywordResult.Keywords);

        // act
        var secondKeywordResult = await _keywordService.CreateKeywordAsync(secondCategory, keyword);

        // assert
        firstCategoryResult.ResultType.Should().Be(ResultType.Created);
        secondCategoryResult.ResultType.Should().Be(ResultType.Created);
        firstKeywordResult.ResultType.Should().Be(ResultType.Created);
        secondKeywordResult.ResultType.Should().Be(ResultType.Created);
        AssertCategoryExists(secondKeywordResult.Keywords, firstCategory);
        AssertCategoryExists(secondKeywordResult.Keywords, secondCategory);
        AssertKeywordExists(secondKeywordResult.Keywords, firstCategory, keyword);
        AssertKeywordExists(secondKeywordResult.Keywords, secondCategory, keyword);
    }

    [TestMethod]
    public async Task CreateKeyword_PreventsDuplicateKeywordInSameCategory()
    {
        // arrange
        var category = Guid.NewGuid().ToString();
        var keyword = Guid.NewGuid().ToString();
        var categoryResult = await _keywordService.CreateCategoryAsync(category);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(categoryResult.Keywords);
        var firstKeywordResult = await _keywordService.CreateKeywordAsync(category, keyword);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(firstKeywordResult.Keywords);

        // act
        var secondKeywordResult = await _keywordService.CreateKeywordAsync(category, keyword);

        // assert
        categoryResult.ResultType.Should().Be(ResultType.Created);
        firstKeywordResult.ResultType.Should().Be(ResultType.Created);
        secondKeywordResult.ResultType.Should().Be(ResultType.Duplicate);
        AssertCategoryExists(secondKeywordResult.Keywords, category);
        AssertKeywordExists(secondKeywordResult.Keywords, category, keyword);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task DeleteCategory_ValidatesArgs(string categoryName)
    {
        // arrange
        var task = async () => { await _keywordService.DeleteCategoryAsync(categoryName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(categoryName));
    }

    [TestMethod]
    public async Task DeleteCategory_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";
        var createResult = await _keywordService.CreateCategoryAsync(categoryName);

        // act
        var deleteResult = await _keywordService.DeleteCategoryAsync(categoryName);

        // assert
        createResult.ResultType.Should().Be(ResultType.Created);
        deleteResult.ResultType.Should().Be(ResultType.Deleted);
        AssertCategoryDoesNotExist(deleteResult.Keywords, categoryName);
        AssertCategoryDoesNotExist(deleteResult.Keywords, categoryNameClean);
    }

    [TestMethod]
    public async Task DeleteCategory_HtmlEncodesInput()
    {
        // arrange
        const string categoryName = "<script>alert('hi');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);
        var createResult = await _keywordService.CreateCategoryAsync(categoryName);

        // act
        var deleteResult = await _keywordService.DeleteCategoryAsync(categoryName);

        // assert
        createResult.ResultType.Should().Be(ResultType.Created);
        deleteResult.ResultType.Should().Be(ResultType.Deleted);
        AssertCategoryDoesNotExist(deleteResult.Keywords, categoryName);
        AssertCategoryDoesNotExist(deleteResult.Keywords, categoryNameClean);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task DeleteKeyword_ValidatesCategoryArg(string categoryName)
    {
        // arrange
        var task = async () => { await _keywordService.DeleteKeywordAsync(categoryName, "keyword"); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(categoryName));
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task DeleteKeyword_ValidatesKeywordArg(string keywordName)
    {
        // arrange
        var task = async () => { await _keywordService.DeleteKeywordAsync("category", keywordName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(keywordName));
    }

    [TestMethod]
    public async Task DeleteKeyword_TrimsInput()
    {
        // arrange
        var categoryNameClean = Guid.NewGuid().ToString();
        var categoryName = $" {categoryNameClean}  ";
        var keywordNameClean = Guid.NewGuid().ToString();
        var keywordName = $" {keywordNameClean}  ";
        var createCategoryResult = await _keywordService.CreateCategoryAsync(categoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createCategoryResult.Keywords);
        var createKeywordResult = await _keywordService.CreateKeywordAsync(categoryName, keywordName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createKeywordResult.Keywords);

        // act
        var deleteResult = await _keywordService.DeleteKeywordAsync(categoryName, keywordName);

        // assert
        createCategoryResult.ResultType.Should().Be(ResultType.Created);
        createKeywordResult.ResultType.Should().Be(ResultType.Created);
        deleteResult.ResultType.Should().Be(ResultType.Deleted);
        AssertCategoryExists(deleteResult.Keywords, categoryNameClean);
        AssertCategoryDoesNotExist(deleteResult.Keywords, categoryName);
        AssertKeywordDoesNotExist(deleteResult.Keywords, categoryNameClean, keywordName);
        AssertKeywordDoesNotExist(deleteResult.Keywords, categoryNameClean, keywordNameClean);
    }

    [TestMethod]
    public async Task DeleteKeyword_HtmlEncodesInput()
    {
        // arrange
        const string categoryName = "<script>alert('hi');</script>";
        const string keywordName = "<script>alert('world');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);
        var keywordNameClean = HttpUtility.HtmlEncode(keywordName);
        var createCategoryResult = await _keywordService.CreateCategoryAsync(categoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createCategoryResult.Keywords);
        var createKeywordResult = await _keywordService.CreateKeywordAsync(categoryName, keywordName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createKeywordResult.Keywords);

        // act
        var deleteResult = await _keywordService.DeleteKeywordAsync(categoryName, keywordName);

        // assert
        createCategoryResult.ResultType.Should().Be(ResultType.Created);
        createKeywordResult.ResultType.Should().Be(ResultType.Created);
        deleteResult.ResultType.Should().Be(ResultType.Deleted);
        AssertCategoryExists(deleteResult.Keywords, categoryNameClean);
        AssertCategoryDoesNotExist(deleteResult.Keywords, categoryName);
        AssertKeywordDoesNotExist(deleteResult.Keywords, categoryNameClean, keywordName);
        AssertKeywordDoesNotExist(deleteResult.Keywords, categoryNameClean, keywordNameClean);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditCategory_ValidatesOldCategoryArg(string oldCategoryName)
    {
        // arrange
        var task = async () => { await _keywordService.EditCategoryAsync(oldCategoryName, "newCategory"); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(oldCategoryName));
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditCategory_ValidatesNewCategoryArg(string newCategoryName)
    {
        // arrange
        var task = async () => { await _keywordService.EditCategoryAsync("oldCategory", newCategoryName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(newCategoryName));
    }

    [TestMethod]
    public async Task EditCategory_TrimsInput()
    {
        // arrange
        var oldCategoryNameClean = Guid.NewGuid().ToString();
        var oldCategoryName = $" {oldCategoryNameClean}  ";
        var newCategoryNameClean = Guid.NewGuid().ToString();
        var newCategoryName = $" {newCategoryNameClean}  ";
        var createResult = await _keywordService.CreateCategoryAsync(oldCategoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createResult.Keywords);

        // act
        var editResult = await _keywordService.EditCategoryAsync(oldCategoryName, newCategoryName);

        // assert
        createResult.ResultType.Should().Be(ResultType.Created);
        editResult.ResultType.Should().Be(ResultType.Edited);
        AssertCategoryDoesNotExist(editResult.Keywords, oldCategoryNameClean);
        AssertCategoryDoesNotExist(editResult.Keywords, oldCategoryName);
        AssertCategoryDoesNotExist(editResult.Keywords, newCategoryName);
        AssertCategoryExists(editResult.Keywords, newCategoryNameClean);
    }

    [TestMethod]
    public async Task EditCategory_HtmlEncodesInput()
    {
        // arrange
        const string oldCategoryName = "<script>alert('hi');</script>";
        const string newCategoryName = "<script>alert('world');</script>";
        var oldCategoryNameClean = HttpUtility.HtmlEncode(oldCategoryName);
        var newCategoryNameClean = HttpUtility.HtmlEncode(newCategoryName);
        var createResult = await _keywordService.CreateCategoryAsync(oldCategoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createResult.Keywords);

        // act
        var editResult = await _keywordService.EditCategoryAsync(oldCategoryName, newCategoryName);

        // assert
        createResult.ResultType.Should().Be(ResultType.Created);
        editResult.ResultType.Should().Be(ResultType.Edited);
        AssertCategoryDoesNotExist(editResult.Keywords, oldCategoryNameClean);
        AssertCategoryDoesNotExist(editResult.Keywords, oldCategoryName);
        AssertCategoryDoesNotExist(editResult.Keywords, newCategoryName);
        AssertCategoryExists(editResult.Keywords, newCategoryNameClean);
    }

    [TestMethod]
    public async Task EditCategory_PreventsDuplicateCategory()
    {
        // arrange
        var oldCategoryName = Guid.NewGuid().ToString();
        var newCategoryName = Guid.NewGuid().ToString();
        var oldCreateResult = await _keywordService.CreateCategoryAsync(oldCategoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(oldCreateResult.Keywords);
        var newCreateResult = await _keywordService.CreateCategoryAsync(newCategoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(newCreateResult.Keywords);

        // act
        var editResult = await _keywordService.EditCategoryAsync(oldCategoryName, newCategoryName);

        // assert
        oldCreateResult.ResultType.Should().Be(ResultType.Created);
        newCreateResult.ResultType.Should().Be(ResultType.Created);
        editResult.ResultType.Should().Be(ResultType.Duplicate);
        AssertCategoryExists(editResult.Keywords, oldCategoryName);
        AssertCategoryExists(editResult.Keywords, newCategoryName);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditKeyword_ValidatesCategoryArg(string categoryName)
    {
        // arrange
        var task = async () => { await _keywordService.EditKeywordAsync(categoryName, "oldKeyword", "newKeyword"); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(categoryName));
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditKeyword_ValidatesOldKeywordArg(string oldKeywordName)
    {
        // arrange
        var task = async () => { await _keywordService.EditKeywordAsync("category", oldKeywordName, "newKeyword"); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(oldKeywordName));
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task EditCategory_ValidatesNewKeywordArg(string newKeywordName)
    {
        // arrange
        var task = async () => { await _keywordService.EditKeywordAsync("category", "oldKeyword", newKeywordName); };

        // act/assert
        await task.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(newKeywordName));
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
        var createCategoryResult = await _keywordService.CreateCategoryAsync(categoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createCategoryResult.Keywords);
        var createOldKeywordResult = await _keywordService.CreateKeywordAsync(categoryName, oldKeywordName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createOldKeywordResult.Keywords);

        // act
        var editResult = await _keywordService.EditKeywordAsync(categoryName, oldKeywordName, newKeywordName);

        // assert
        createCategoryResult.ResultType.Should().Be(ResultType.Created);
        createOldKeywordResult.ResultType.Should().Be(ResultType.Created);
        editResult.ResultType.Should().Be(ResultType.Edited);
        AssertCategoryDoesNotExist(editResult.Keywords, categoryName);
        AssertCategoryExists(editResult.Keywords, categoryNameClean);
        AssertKeywordDoesNotExist(editResult.Keywords, categoryNameClean, oldKeywordName);
        AssertKeywordDoesNotExist(editResult.Keywords, categoryNameClean, oldKeywordNameClean);
        AssertKeywordDoesNotExist(editResult.Keywords, categoryNameClean, newKeywordName);
        AssertKeywordExists(editResult.Keywords, categoryNameClean, newKeywordNameClean);
    }

    [TestMethod]
    public async Task EditKeyword_HtmlEncodesInput()
    {
        // arrange
        const string categoryName = "<script>alert('foo');</script>";
        const string oldKeywordName = "<script>alert('bar');</script>";
        const string newKeywordName = "<script>alert('baz');</script>";
        var categoryNameClean = HttpUtility.HtmlEncode(categoryName);
        var oldKeywordNameClean = HttpUtility.HtmlEncode(oldKeywordName);
        var newKeywordNameClean = HttpUtility.HtmlEncode(newKeywordName);
        var createCategoryResult = await _keywordService.CreateCategoryAsync(categoryName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createCategoryResult.Keywords);
        var createOldKeywordResult = await _keywordService.CreateKeywordAsync(categoryName, oldKeywordName);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(createOldKeywordResult.Keywords);

        // act
        var editResult = await _keywordService.EditKeywordAsync(categoryName, oldKeywordName, newKeywordName);

        // assert
        createCategoryResult.ResultType.Should().Be(ResultType.Created);
        createOldKeywordResult.ResultType.Should().Be(ResultType.Created);
        editResult.ResultType.Should().Be(ResultType.Edited);
        AssertCategoryDoesNotExist(editResult.Keywords, categoryName);
        AssertCategoryExists(editResult.Keywords, categoryNameClean);
        AssertKeywordDoesNotExist(editResult.Keywords, categoryNameClean, oldKeywordName);
        AssertKeywordDoesNotExist(editResult.Keywords, categoryNameClean, oldKeywordNameClean);
        AssertKeywordDoesNotExist(editResult.Keywords, categoryNameClean, newKeywordName);
        AssertKeywordExists(editResult.Keywords, categoryNameClean, newKeywordNameClean);
    }

    [TestMethod]
    public async Task EditKeyword_AllowsDuplicateKeywordInDifferentCategory()
    {
        // arrange
        var firstCategory = Guid.NewGuid().ToString();
        var secondCategory = Guid.NewGuid().ToString();
        var firstKeyword = Guid.NewGuid().ToString();
        var secondKeyword = Guid.NewGuid().ToString();
        var firstCategoryResult = await _keywordService.CreateCategoryAsync(firstCategory);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(firstCategoryResult.Keywords);
        var secondCategoryResult = await _keywordService.CreateCategoryAsync(secondCategory);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(secondCategoryResult.Keywords);
        var firstKeywordResult = await _keywordService.CreateKeywordAsync(firstCategory, firstKeyword);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(firstKeywordResult.Keywords);
        var secondKeywordResult = await _keywordService.CreateKeywordAsync(secondCategory, secondKeyword);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(secondKeywordResult.Keywords);

        // act
        var editResult = await _keywordService.EditKeywordAsync(firstCategory, firstKeyword, secondKeyword);

        // assert
        firstCategoryResult.ResultType.Should().Be(ResultType.Created);
        secondCategoryResult.ResultType.Should().Be(ResultType.Created);
        firstKeywordResult.ResultType.Should().Be(ResultType.Created);
        secondKeywordResult.ResultType.Should().Be(ResultType.Created);
        editResult.ResultType.Should().Be(ResultType.Edited);
        AssertCategoryExists(editResult.Keywords, firstCategory);
        AssertCategoryExists(editResult.Keywords, secondCategory);
        AssertKeywordDoesNotExist(editResult.Keywords, firstCategory, firstKeyword);
        AssertKeywordExists(editResult.Keywords, firstCategory, secondKeyword);
        AssertKeywordExists(editResult.Keywords, secondCategory, secondKeyword);
    }

    [TestMethod]
    public async Task EditKeyword_PreventsDuplicateKeywordInSameCategory()
    {
        // arrange
        var category = Guid.NewGuid().ToString();
        var firstKeyword = Guid.NewGuid().ToString();
        var secondKeyword = Guid.NewGuid().ToString();
        var categoryResult = await _keywordService.CreateCategoryAsync(category);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(categoryResult.Keywords);
        var firstKeywordResult = await _keywordService.CreateKeywordAsync(category, firstKeyword);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(firstKeywordResult.Keywords);
        var secondKeywordResult = await _keywordService.CreateKeywordAsync(category, secondKeyword);
        _mockKeywordRepository.Setup(mock => mock.ReadAsync()).ReturnsAsync(secondKeywordResult.Keywords);

        // act
        var editResult = await _keywordService.EditKeywordAsync(category, firstKeyword, secondKeyword);

        // assert
        categoryResult.ResultType.Should().Be(ResultType.Created);
        firstKeywordResult.ResultType.Should().Be(ResultType.Created);
        secondKeywordResult.ResultType.Should().Be(ResultType.Created);
        editResult.ResultType.Should().Be(ResultType.Duplicate);
        AssertCategoryExists(editResult.Keywords, category);
        AssertKeywordExists(editResult.Keywords, category, firstKeyword);
        AssertKeywordExists(editResult.Keywords, category, secondKeyword);
    }

    [TestMethod]
    public async Task CanGetKeywords()
    {
        // act
        var keywords = (await _keywordService.GetKeywordsAsync()).ToArray();

        // assert
        keywords.Length.Should().BeGreaterThan(0);
        foreach (var keyword in keywords)
        {
            Assert.IsNotNull(keyword);
            keyword.Should().NotBeNull();
            keyword.Name.Should().NotBeNullOrWhiteSpace();
            keyword.Children.Should().NotBeNull();
            keyword.Children.Count.Should().BeGreaterThan(0);
            foreach (var child in keyword.Children)
            {
                child.Should().NotBeNull();
                child.Name.Should().NotBeNullOrWhiteSpace();
                child.Children.Should().NotBeNull();
                child.Children.Should().BeEmpty();
            }
        }
    }

    private static void AssertCategoryExists(IEnumerable<Keyword> keywords, string categoryName)
    {
        keywords.Should().Contain(item =>
            item.IsCategory &&
            item.Name.Equals(categoryName, StringComparison.Ordinal));
    }

    private static void AssertCategoryDoesNotExist(IEnumerable<Keyword> keywords, string categoryName)
    {
        keywords.Should().NotContain(item =>
            item.Name.Equals(categoryName, StringComparison.Ordinal));
    }

    private static void AssertKeywordExists(IEnumerable<Keyword> keywords, string categoryName, string keywordName)
    {
        keywords.Should().Contain(item =>
            item.IsCategory &&
            item.Name.Equals(categoryName) &&
            item.Children.Count(keyword => 
                !keyword.IsCategory &&
                keyword.Name.Equals(keywordName, StringComparison.Ordinal) &&
                !keyword.Children.Any()) == 1);
    }

    private static void AssertKeywordDoesNotExist(IEnumerable<Keyword> keywords, string categoryName, string keywordName)
    {
        keywords.Should().Contain(item =>
            item.IsCategory &&
            item.Name.Equals(categoryName) &&
            !item.Children.Any(keyword =>
                keyword.Name.Equals(keywordName, StringComparison.Ordinal)));
    }
}