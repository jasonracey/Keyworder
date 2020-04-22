using System.IO;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using KeyworderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeyworderLibTest
{
    [TestClass]
    public class WhenCreatingKeywords
    {
        private string path;
        private KeywordRepository keywordRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            path = TestData.GetTestPath();
            File.WriteAllText(path, TestData.GetTestData());
            keywordRepository = new KeywordRepository(path);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        [TestMethod]
        public void CanCreateNewCategory()
        {
            // arrange
            const string testCategoryId = "TestCategory";

            // act
            keywordRepository.CreateCategory(testCategoryId);

            // assert
            keywordRepository.GetCategories().Should()
                .Contain(c => c.CategoryId == testCategoryId);
        }

        [TestMethod]
        public void CannotCreateDuplicateCategory()
        {
            // arrange
            var categoryWithAtLeastOneKeyword = keywordRepository.GetCategories()
                .First(c => c.Keywords.Count > 0);
            var keywordCount = categoryWithAtLeastOneKeyword.Keywords.Count;
            var testCategoryId = categoryWithAtLeastOneKeyword.CategoryId;

            // act
            keywordRepository.CreateCategory(testCategoryId);

            // assert
            var unchangedCategory = keywordRepository.GetCategories()
                .Single(c => c.CategoryId == testCategoryId);
            unchangedCategory.Keywords.Count.Should().Be(keywordCount);

            // also check file because sorted set only contains distinct items
            XDocument.Load(path)
                .Descendants("Category")
                .Count(e => e.Attribute("CategoryId").Value == testCategoryId)
                .Should().Be(1);
        }

        [TestMethod]
        public void CanCreateNewKeyword()
        {
            // arrange
            var testCategoryId = keywordRepository.GetCategories().First().CategoryId;
            const string testKeywordId = "TestKeyword";

            // act
            keywordRepository.CreateKeyword(testCategoryId, testKeywordId);

            // assert
            keywordRepository.GetCategories().Single(c => c.CategoryId == testCategoryId)
                .Keywords.Should().Contain(k => k.KeywordId == testKeywordId);
        }

        [TestMethod]
        public void CanCreateDuplicateKeywordInDifferentCategory()
        {
            // arrange
            var categories = keywordRepository.GetCategories();
            var existingKeywordId = categories.First(c => c.Keywords.Count > 0)
                .Keywords.First().KeywordId;
            var differentCategoryId = categories.First(c => c.Keywords
                .All(k => k.KeywordId != existingKeywordId)).CategoryId;

            // act
            keywordRepository.CreateKeyword(differentCategoryId, existingKeywordId);

            // assert
            keywordRepository.GetCategories().Single(c => c.CategoryId == differentCategoryId)
                .Keywords.Should().Contain(k => k.KeywordId == existingKeywordId);
        }

        [TestMethod]
        public void CannotCreateDuplicateKeywordWithinCategory()
        {
            // arrange
            var categories = keywordRepository.GetCategories();
            var testCategory = categories.First(c => c.Keywords.Count > 0);
            var testCategoryId = testCategory.CategoryId;
            var existingKeywordId = testCategory.Keywords.First().KeywordId;

            // act
            keywordRepository.CreateKeyword(testCategoryId, existingKeywordId);

            // assert
            XDocument.Load(path)
                .Descendants("Category").Single(e => e.Attribute("CategoryId").Value == testCategoryId)
                .Descendants("Keyword").Count(e => e.Attribute("KeywordId").Value == existingKeywordId)
                .Should().Be(1);
        }
    }
}
