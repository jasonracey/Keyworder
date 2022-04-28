using Keyworder.Data;
using Keyworder.Utilities;
using Radzen;

namespace Keyworder;

public static class ServiceCollectionExtensions
{
    public static void AddKeyworderServices(
        this IServiceCollection services, 
        ConfigurationManager configuration)
    {
        services
            .AddClipboardService()
            .AddKeywordService(configuration)
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
        IConfiguration configuration)
    {
        // Add as singleton so all clients use the same file lock instance
        var keywordsJsonPath = configuration["KeywordsJsonPath"];
        var keywordService = new KeywordService(keywordsJsonPath);
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
