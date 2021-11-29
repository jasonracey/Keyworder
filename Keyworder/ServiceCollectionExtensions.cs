using Keyworder.Data;

namespace Keyworder;

public static class ServiceCollectionExtensions
{
    public static void AddKeyworderServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var keywordsXmlPath = configuration["KeywordsXmlPath"];
        var keywordService = new KeywordService(keywordsXmlPath);
        services.AddSingleton(keywordService);
    }
}
