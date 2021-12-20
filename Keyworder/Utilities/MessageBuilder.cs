using Radzen;

namespace Keyworder.Utilities
{
    public static class MessageBuilder
    {
        public static NotificationMessage WithDetail(
            this NotificationMessage message, 
            string? detail)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return new NotificationMessage
            {
                Detail = detail,
                Duration = message.Duration,
                Severity = message.Severity,
                Summary = message.Summary
            };
        }

        private static readonly NotificationMessage baseSuccessMessage = new NotificationMessage
        {
            Duration = 2000,
            Severity = NotificationSeverity.Success,
            Summary = "Success"
        };

        private static readonly NotificationMessage baseDuplicateMessage = new NotificationMessage
        {
            Duration = 2000,
            Severity = NotificationSeverity.Warning,
            Summary = "Duplicate"
        };

        private static readonly NotificationMessage baseErrorMessage = new NotificationMessage
        {
            Detail = "Sorry there was an error",
            Duration = 2000,
            Severity = NotificationSeverity.Error,
            Summary = "Error"
        };

        public static NotificationMessage BuildCreatedMessage(EntityType entityType)
        {
            return baseSuccessMessage
                .WithDetail($"{entityType} created successfully");
        }

        public static NotificationMessage BuildDeletedMessage(EntityType entityType)
        {
            return baseSuccessMessage
                .WithDetail($"{entityType} deleted successfully");
        }

        public static NotificationMessage BuildDuplicateMessage(EntityType entityType)
        {
            return baseDuplicateMessage
                .WithDetail($"{entityType} already exists");
        }

        public static NotificationMessage BuildEditedMessage(EntityType entityType)
        {
            return baseSuccessMessage
                .WithDetail($"{entityType} edited successfully");
        }

        public static NotificationMessage BuildErrorMessage()
        {
            return baseErrorMessage;
        }
    }
}
