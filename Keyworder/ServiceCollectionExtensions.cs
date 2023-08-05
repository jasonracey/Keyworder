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
            .AddScoped<ClipboardService>()
            .AddSingleton<NotificationService>()
            .AddSingleton<StateContainer>();
    }

    private static IServiceCollection AddKeywordService(
        this IServiceCollection services,
        string storageAccountConnectionString,
        ILogger<KeywordService> logger)
    {
        var keywordRepository = new BlobKeywordRepository(storageAccountConnectionString);
        var keywordService = new KeywordService(keywordRepository, logger);
        return services.AddSingleton(keywordService);
    }
}
