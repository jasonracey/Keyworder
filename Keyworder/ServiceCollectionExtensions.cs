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
            .AddKeywordService(storageAccountConnectionString, logger)
            .AddSingleton<NotificationService>()
            .AddScoped<ClipboardService>();
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
}
