//using System;
//using System.Linq;
//using FluentAssertions;
//using KeyworderLib;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace KeyworderLibTest
//{
//    [TestClass]
//    public class WhenUpdatingKeywords
//    {
//        [TestInitialize]
//        public void TestInitialize()
//        {
//            TestData.Create();
//        }

//        [TestMethod]
//        public void CanUpdateCategory()
//        {
//            // arrange
//            const string newCategoryId = "TestCategory";
//            var oldCategories = KeywordRepository.GetCategories();
//            oldCategories.Should().NotContain(c => c.CategoryId == newCategoryId);
//            var oldCategoryId = oldCategories.First().CategoryId;
//            oldCategoryId.Should().NotBe(newCategoryId);

//            // act
//            KeywordRepository.UpdateCategory(oldCategoryId, newCategoryId);

//            // assert
//            var newCategories = KeywordRepository.GetCategories();
//            newCategories.Should().NotContain(c => c.CategoryId == oldCategoryId);
//            newCategories.Count(c => c.CategoryId == newCategoryId).Should().Be(1);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ArgumentException))]
//        public void ExceptionThrownWhenCategoryDoesntExist()
//        {
//            // arrange
//            const string oldCategoryId = "Foo";
//            const string newCategoryId = "Bar";
//            KeywordRepository.GetCategories().Count(c => c.CategoryId == oldCategoryId).Should().Be(0);

//            // act/assert
//            KeywordRepository.UpdateCategory(oldCategoryId, newCategoryId);
//        }

//        [TestMethod]
//        public void CanUpdateKeyword()
//        {
//            // arrange
//            const string newKeywordId = "TestKeyword";
//            var category = KeywordRepository.GetCategories().First();
//            var categoryId = category.CategoryId;
//            var oldKeywordId = category.Keywords.First().KeywordId;
//            category.Keywords.Count(k => k.KeywordId == oldKeywordId).Should().Be(1);
//            category.Keywords.Should().NotContain(c => c.KeywordId == newKeywordId);
//            oldKeywordId.Should().NotBe(newKeywordId);

//            // act
//            KeywordRepository.UpdateKeyword(categoryId, oldKeywordId, newKeywordId);

//            // assert
//            var newCategories = KeywordRepository.GetCategories();
//            newCategories.Single(c => c.CategoryId == categoryId)
//                .Keywords.Count(k => k.KeywordId == newKeywordId)
//                .Should().Be(1);
//            newCategories.Single(c => c.CategoryId == categoryId)
//                .Keywords.Should()
//                .NotContain(k => k.KeywordId == oldKeywordId);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ArgumentException))]
//        public void ExceptionThrownWhenKeywordDoesntExist()
//        {
//            // arrange
//            const string oldKeywordId = "Foo";
//            const string newKeywordId = "Bar";
//            var cateogry = KeywordRepository.GetCategories().First();
//            cateogry.Keywords.Count(k => k.KeywordId == oldKeywordId).Should().Be(0);

//            // act/assert
//            KeywordRepository.UpdateKeyword(cateogry.CategoryId, oldKeywordId, newKeywordId);
//        }
//    }
//}
