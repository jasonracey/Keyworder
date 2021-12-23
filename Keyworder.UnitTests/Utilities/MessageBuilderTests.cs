using FluentAssertions;
using Keyworder.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radzen;

namespace Keyworder.UnitTests.Utilities
{
    [TestClass]
    public class MessageBuilderTests
    {
        [TestMethod]
        public void WhenGettingCopySuccessMessage_ReturnsExpectedMessage()
        {
            // act
            var message = MessageBuilder.BuildCopySuccessMessage();

            // assert
            message.Severity.Should().Be(NotificationSeverity.Success);
        }

        [TestMethod]
        public void WhenGettingCopyWarningMessage_ReturnsExpectedMessage()
        {
            // act
            var message = MessageBuilder.BuildCopyWarningMessage();

            // assert
            message.Severity.Should().Be(NotificationSeverity.Warning);
        }

        [DataTestMethod]
        [DataRow(EntityType.Category)]
        [DataRow(EntityType.Keyword)]
        public void WhenGettingCreatedMessage_ReturnsExpectedMessage(EntityType entityType)
        {
            // act
            var message = MessageBuilder.BuildCreatedMessage(entityType);

            // assert
            message.Severity.Should().Be(NotificationSeverity.Success);
            message.Detail.Should().Contain(entityType.ToString());
        }

        [DataTestMethod]
        [DataRow(EntityType.Category)]
        [DataRow(EntityType.Keyword)]
        public void WhenGettingDeletedMessage_ReturnsExpectedMessage(EntityType entityType)
        {
            // act
            var message = MessageBuilder.BuildDeletedMessage(entityType);

            // assert
            message.Severity.Should().Be(NotificationSeverity.Success);
            message.Detail.Should().Contain(entityType.ToString());
        }

        [DataTestMethod]
        [DataRow(EntityType.Category)]
        [DataRow(EntityType.Keyword)]
        public void WhenGettingDuplicateMessage_ReturnsExpectedMessage(EntityType entityType)
        {
            // act
            var message = MessageBuilder.BuildDuplicateMessage(entityType);

            // assert
            message.Severity.Should().Be(NotificationSeverity.Warning);
            message.Detail.Should().Contain(entityType.ToString());
        }

        [DataTestMethod]
        [DataRow(EntityType.Category)]
        [DataRow(EntityType.Keyword)]
        public void WhenGettingEditedMessage_ReturnsExpectedMessage(EntityType entityType)
        {
            // act
            var message = MessageBuilder.BuildEditedMessage(entityType);

            // assert
            message.Severity.Should().Be(NotificationSeverity.Success);
            message.Detail.Should().Contain(entityType.ToString());
        }

        [TestMethod]
        public void WhenGettingErrorMessage_ReturnsExpectedMessage()
        {
            // act
            var message = MessageBuilder.BuildErrorMessage();

            // assert
            message.Severity.Should().Be(NotificationSeverity.Error);
        }
    }
}
