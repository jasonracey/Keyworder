//using System.Linq;
//using FluentAssertions;
//using KeyworderLib;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace KeyworderLibTest
//{
//    [TestClass]
//    public class WhenDeletingKeywords
//    {
//        [TestInitialize]
//        public void TestInitialize()
//        {
//            TestData.Create();
//        }

//        [TestMethod]
//        public void CanDeleteExistingCategory()
//        {
//            // arrange
//            var testCategoryId = KeywordRepository.GetCategories().First().CategoryId;

//            // act
//            KeywordRepository.DeleteCategory(testCategoryId);

//            // assert
//            KeywordRepository.GetCategories().Should()
//                .NotContain(c => c.CategoryId == testCategoryId);
//        }
        
//        [TestMethod]
//        public void NoErrorWhenCategoryDoesntExist()
//        {
//            // arrange
//            const string testCategoryId = "Foo";

//            // act
//            KeywordRepository.DeleteCategory(testCategoryId);

//            // assert
//            KeywordRepository.GetCategories().Should()
//                .NotContain(c => c.CategoryId == testCategoryId);
//        }

//        [TestMethod]
//        public void CanDeleteExistingKeyword()
//        {
//            // arrange
//            var testCategory = KeywordRepository.GetCategories().First();
//            var testCategoryId = testCategory.CategoryId;
//            var testKeywordId = testCategory.Keywords.First().KeywordId;

//            // act
//            KeywordRepository.DeleteKeyword(testCategoryId, testKeywordId);

//            // assert
//            KeywordRepository.GetCategories().Single(c => c.CategoryId == testCategoryId)
//                .Keywords.Should().NotContain(k => k.KeywordId == testKeywordId);
//        }

//        [TestMethod]
//        public void NoErrorWhenKeywordDoesntExist()
//        {
//            // arrange
//            var testCategory = KeywordRepository.GetCategories().First();
//            var testCategoryId = testCategory.CategoryId;
//            const string testKeywordId = "Foo";

//            // act
//            KeywordRepository.DeleteKeyword(testCategoryId, testKeywordId);

//            // assert
//            KeywordRepository.GetCategories().Single(c => c.CategoryId == testCategoryId)
//                .Keywords.Should().NotContain(k => k.KeywordId == testKeywordId);
//        }
//    }
//}
