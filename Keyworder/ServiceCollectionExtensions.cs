using Keyworder.Data;

namespace Keyworder;

public static class ServiceCollectionExtensions
{
    public static void AddKeyworderServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var keywordsJsonPath = configuration["KeywordsJsonPath"];
        var keywordService = new KeywordService(keywordsJsonPath);
        services.AddSingleton(keywordService);
    }
}
