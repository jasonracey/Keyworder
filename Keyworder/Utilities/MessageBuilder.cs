using Radzen;

namespace Keyworder.Utilities
{
    public static class MessageBuilder
    {
        private static readonly NotificationMessage BaseSuccessMessage = new()
        {
            Duration = 2000,
            Severity = NotificationSeverity.Success,
            Summary = "Success"
        };

        private static readonly NotificationMessage BaseWarningMessage = new()
        {
            Duration = 2000,
            Severity = NotificationSeverity.Warning,
            Summary = "Warning"
        };

        private static readonly NotificationMessage BaseErrorMessage = new()
        {
            Detail = "Sorry there was an error",
            Duration = 2000,
            Severity = NotificationSeverity.Error,
            Summary = "Error"
        };

        public static NotificationMessage BuildCopySuccessMessage()
        {
            return BaseSuccessMessage
                .WithDetail($"Keywords copied to clipboard");
        }

        public static NotificationMessage BuildCopyWarningMessage()
        {
            return BaseWarningMessage
                .WithDetail($"Please select at least one keyword to copy to clipboard");
        }

        public static NotificationMessage BuildCreatedMessage(EntityType entityType)
        {
            return BaseSuccessMessage
                .WithDetail($"{entityType} created");
        }

        public static NotificationMessage BuildDeletedMessage(EntityType entityType)
        {
            return BaseSuccessMessage
                .WithDetail($"{entityType} deleted");
        }

        public static NotificationMessage BuildDuplicateMessage(EntityType entityType)
        {
            return BaseWarningMessage
                .WithDetail($"{entityType} already exists");
        }

        public static NotificationMessage BuildEditedMessage(EntityType entityType)
        {
            return BaseSuccessMessage
                .WithDetail($"{entityType} edited");
        }

        public static NotificationMessage BuildErrorMessage()
        {
            return BaseErrorMessage;
        }
        
        private static NotificationMessage WithDetail(
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
    }
}
