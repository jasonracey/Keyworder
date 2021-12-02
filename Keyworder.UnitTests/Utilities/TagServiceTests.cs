using Keyworder.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Keyworder.UnitTests.Utilities
{
    [TestClass]
    public class TagServiceTests
    {
        [TestMethod]
        public void WhenCollectionIsNull_ThrowsExpectedException()
        {
            // act/assert
            Assert.ThrowsException<ArgumentNullException>(() => TagService.ToFlickrTagsString(null!));
        }

        [TestMethod]
        public void WhenCollectionIsEmpty_ReturnsEmptyString()
        {
            // arrange
            var values = new List<string>();

            // act
            var result = TagService.ToFlickrTagsString(values);

            // assert
            Assert.AreEqual(string.Empty, result);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void WhenNullOrWhiteSpace_ValueIsExcluded(string value)
        {
            // arrange
            var values = new List<string> { value };

            // act
            var result = TagService.ToFlickrTagsString(values);

            // assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void ValuesAre_Quoted_CommaDelimited_SortedCaseInsensitive()
        {
            // arrange
            var values = new List<string> { "b", "c", "B", "a" };

            // act
            var result = TagService.ToFlickrTagsString(values);

            // assert
            Assert.AreEqual("\"a\",\"b\",\"B\",\"c\"", result);
        }
    }
}