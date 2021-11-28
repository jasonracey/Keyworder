using System.Xml.Linq;

namespace Keyworder.Data;

public class KeywordService
{
    private readonly string keywordsXmlPath;

    private static readonly CategoryComparer categoryComparer = new();

    public KeywordService(string keywordsXmlPath)
    {
        if (string.IsNullOrWhiteSpace(keywordsXmlPath))
            throw new ArgumentNullException(nameof(keywordsXmlPath));

        this.keywordsXmlPath = keywordsXmlPath;

        if (!File.Exists(this.keywordsXmlPath))
            File.Create(this.keywordsXmlPath);
    }

    public async Task CreateKeywordAsync(string categoryId, string keywordId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentNullException(nameof(categoryId));
        if (string.IsNullOrWhiteSpace(keywordId))
            throw new ArgumentNullException(nameof(keywordId));

        var document = XDocument.Load(keywordsXmlPath);

        var category = document.Descendants("Category")
            .Single(d => d?.Attribute("CategoryId")?.Value == categoryId);

        if (category.Descendants("Keyword").Count(d => d?.Attribute("KeywordId")?.Value == keywordId) == 0)
        {
            category.Add(new XElement("Keyword", new XAttribute("KeywordId", keywordId)));
            document.Save(keywordsXmlPath);
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task CreateCategoryAsync(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentNullException(nameof(categoryId));
            
        var document = XDocument.Load(keywordsXmlPath);

        if (document.Descendants("Category").Count(d => d?.Attribute("CategoryId")?.Value == categoryId) == 0)
        {
            if (document.Root == null)
                throw new KeyworderException("Encountered a null root element");

            document.Root.Add(new XElement("Category", new XAttribute("CategoryId", categoryId)));
            document.Save(keywordsXmlPath);
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task DeleteKeywordAsync(string categoryId, string keywordId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentNullException(nameof(categoryId));
        if (string.IsNullOrWhiteSpace(keywordId))
            throw new ArgumentNullException(nameof(keywordId));
            
        var document = XDocument.Load(keywordsXmlPath);

        if (document.Descendants("Category")
                .Single(d => d?.Attribute("CategoryId")?.Value == categoryId)
                .Descendants("Keyword")
                .Count(d => d?.Attribute("KeywordId")?.Value == keywordId) > 0)
        {
            document.Descendants("Category")
                .Single(d => d?.Attribute("CategoryId")?.Value == categoryId)
                .Descendants("Keyword")
                .Single(d => d?.Attribute("KeywordId")?.Value == keywordId)
                .Remove();

            document.Save(keywordsXmlPath);
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task DeleteCategoryAsync(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentNullException(nameof(categoryId));
            
        var document = XDocument.Load(keywordsXmlPath);
            
        if (document.Descendants("Category").Count(d => d?.Attribute("CategoryId")?.Value == categoryId) > 0)
        {
            document.Descendants("Category")
                .Single(d => d?.Attribute("CategoryId")?.Value == categoryId)
                .Remove();

            document.Save(keywordsXmlPath);
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        var document = XDocument.Load(keywordsXmlPath);

        var categoryNodes = document.Descendants("Category");

        var categories = new SortedSet<Category>(categoryComparer);

        foreach (var categoryNode in categoryNodes)
        {
            var categoryId = categoryNode?.Attribute("CategoryId")?.Value ?? 
                throw new KeyworderException("Category node is missing a CategoryId attribute.");

            var category = new Category(categoryId);
            foreach (var keywordNode in categoryNode?.Elements("Keyword") ?? Enumerable.Empty<XElement>())
            {
                var keywordId = keywordNode?.Attribute("KeywordId")?.Value ?? 
                    throw new KeyworderException("Keyword node is missing a KeywordId attribute.");

                category.Keywords.Add(new Keyword(keywordId, categoryId));
            }
            categories.Add(category);
        }

        return await Task.FromResult(categories).ConfigureAwait(false);
    }

    public async Task UpdateKeywordAsync(string categoryId, string oldKeywordId, string newKeywordId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentNullException(nameof(categoryId));
        if (string.IsNullOrWhiteSpace(oldKeywordId))
            throw new ArgumentNullException(nameof(oldKeywordId));
        if (string.IsNullOrWhiteSpace(newKeywordId))
            throw new ArgumentNullException(nameof(newKeywordId));

        var document = XDocument.Load(keywordsXmlPath);

        var keyword = document.Descendants("Category")
            .Single(d => d?.Attribute("CategoryId")?.Value == categoryId)
            .Descendants("Keyword")
            .SingleOrDefault(d => d?.Attribute("KeywordId")?.Value == oldKeywordId);

        if (keyword == null)
            throw new KeyworderException($"Keyword with id {oldKeywordId} not found");

        keyword.SetAttributeValue("KeywordId", newKeywordId);
        document.Save(keywordsXmlPath);

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task UpdateCategoryAsync(string oldCategoryId, string newCategoryId)
    {
        if (string.IsNullOrWhiteSpace(oldCategoryId))
            throw new ArgumentNullException(nameof(oldCategoryId));
        if (string.IsNullOrWhiteSpace(newCategoryId))
            throw new ArgumentNullException(nameof(newCategoryId));

        var document = XDocument.Load(keywordsXmlPath);
            
        var category = document.Descendants("Category")
            .SingleOrDefault(d => d?.Attribute("CategoryId")?
            .Value == oldCategoryId);
            
        if (category == null)
            throw new KeyworderException($"Category with id {oldCategoryId} not found");

        category.SetAttributeValue("CategoryId", newCategoryId);
        document.Save(keywordsXmlPath);

        await Task.CompletedTask.ConfigureAwait(false);
    }
}