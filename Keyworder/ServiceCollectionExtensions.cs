using Keyworder.Data;
using Radzen;

namespace Keyworder;

public static class ServiceCollectionExtensions
{
    public static void AddKeyworderServices(
        this IServiceCollection services, 
        ConfigurationManager configuration)
    {
        services
            .AddKeywordService(configuration)
            .AddNotificationService();
    }

    private static IServiceCollection AddKeywordService(
        this IServiceCollection services, 
        ConfigurationManager configuration)
    {
        var keywordsJsonPath = configuration["KeywordsJsonPath"];
        var keywordService = new KeywordService(keywordsJsonPath);
        return services.AddSingleton(keywordService);
    }

    private static IServiceCollection AddNotificationService(
        this IServiceCollection services)
    {
        var notificationService = new NotificationService();
        return services.AddSingleton(notificationService);
    }
}
