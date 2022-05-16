using Keyworder.Data;
using Keyworder.Utilities;
using Radzen;

namespace Keyworder;

public static class ServiceCollectionExtensions
{
    public static void AddKeyworderServices(
        this IServiceCollection services,
        string storageAccountConnectionString,
        ILogger<KeywordService> logger)
    {
        services
            .AddClipboardService()
            .AddKeywordService(storageAccountConnectionString, logger)
            .AddNotificationService();
    }

    private static IServiceCollection AddClipboardService(
        this IServiceCollection services)
    {
        // Add as scoped so each client has its own clipboard
        return services.AddScoped<ClipboardService>();
    }

    private static IServiceCollection AddKeywordService(
        this IServiceCollection services,
        string storageAccountConnectionString,
        ILogger<KeywordService> logger)
    {
        // Add as singleton because there's no state
        var keywordRepository = new BlobKeywordRepository(storageAccountConnectionString);
        var keywordService = new KeywordService(keywordRepository, logger);
        return services.AddSingleton(keywordService);
    }

    private static IServiceCollection AddNotificationService(
        this IServiceCollection services)
    {
        // Add as singleton because there's no state
        var notificationService = new NotificationService();
        return services.AddSingleton(notificationService);
    }
}
