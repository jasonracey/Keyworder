using System.Web;

namespace Keyworder.Data;

public class KeywordService
{
    private readonly IKeywordRepository _keywordRepository;
    private readonly ILogger<KeywordService> _logger;

    public KeywordService(
        IKeywordRepository keywordRepository,
        ILogger<KeywordService> logger)
    {
        _keywordRepository = keywordRepository ?? throw new ArgumentNullException(nameof(keywordRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<KeywordResult> CreateCategoryAsync(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentNullException(nameof(categoryName));

        var existingKeywords = (await GetKeywordsAsync().ConfigureAwait(false)).ToArray();

        var categoryNameClean = string.Empty;
        
        try
        {
            categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());

            var existingCategory = existingKeywords.FirstOrDefault(keyword => keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

            if (existingCategory != null)
            {
                _logger.LogWarning("CreateCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Duplicate), categoryNameClean);
                return new KeywordResult(ResultType.Duplicate, existingKeywords);
            }

            var newCategory = new Keyword
            {
                Name = categoryNameClean,
                IsCategory = true,
                Children = new List<Keyword>()
            };

            var updatedKeywords = existingKeywords.Concat(new[] {newCategory});
            
            var newKeywords = await _keywordRepository.WriteAsync(updatedKeywords).ConfigureAwait(false);

            _logger.LogInformation("CreateCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Created), categoryNameClean);
            return new KeywordResult(ResultType.Created, newKeywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Error), categoryNameClean);
            return new KeywordResult(ResultType.Error, existingKeywords);
        }
    }

    public async Task<KeywordResult> CreateKeywordAsync(string? categoryName, string? keywordName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentNullException(nameof(categoryName));
        if (string.IsNullOrWhiteSpace(keywordName))
            throw new ArgumentNullException(nameof(keywordName));

        var existingKeywords = (await GetKeywordsAsync().ConfigureAwait(false))
            .ToArray();

        var categoryNameClean = string.Empty;
        var keywordNameClean = string.Empty;

        try
        {
            categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());
            keywordNameClean = HttpUtility.HtmlEncode(keywordName.Trim());

            var existingCategory = existingKeywords.Single(keyword => keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

            if (existingCategory.Children.Any(keyword => keyword.Name.Equals(keywordNameClean, StringComparison.Ordinal)))
            {
                _logger.LogWarning("CreateKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Duplicate), categoryNameClean, keywordNameClean);
                return new KeywordResult(ResultType.Duplicate, existingKeywords);
            }

            var updatedCategory = new Keyword
            {
                Name = existingCategory.Name,
                IsCategory = existingCategory.IsCategory,
                Children = existingCategory.Children
                    .Concat(new[] { new Keyword { Name = keywordNameClean } })
                    .ToList()
            };

            var updatedKeywords = existingKeywords
                .Except(new[] { existingCategory })
                .Concat(new[] { updatedCategory });

            var newKeywords = await _keywordRepository.WriteAsync(updatedKeywords).ConfigureAwait(false);

            _logger.LogInformation("CreateKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Created), categoryNameClean, keywordNameClean);
            return new KeywordResult(ResultType.Created, newKeywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Error), categoryNameClean, keywordNameClean);
            return new KeywordResult(ResultType.Error, existingKeywords);
        }
    }

    public async Task<KeywordResult> DeleteCategoryAsync(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentNullException(nameof(categoryName));

        var existingKeywords = (await GetKeywordsAsync().ConfigureAwait(false)).ToArray();

        var categoryNameClean = string.Empty;

        try
        {
            categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());

            var updatedKeywords = existingKeywords.Where(keyword => !keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

            var newKeywords = await _keywordRepository.WriteAsync(updatedKeywords).ConfigureAwait(false);

            _logger.LogInformation("DeleteCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Deleted), categoryNameClean);
            return new KeywordResult(ResultType.Deleted, newKeywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Error), categoryNameClean);
            return new KeywordResult(ResultType.Error, existingKeywords);
        }
    }

    public async Task<KeywordResult> DeleteKeywordAsync(string? categoryName, string? keywordName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentNullException(nameof(categoryName));
        if (string.IsNullOrWhiteSpace(keywordName))
            throw new ArgumentNullException(nameof(keywordName));

        var existingKeywords = (await GetKeywordsAsync().ConfigureAwait(false)).ToArray();

        var categoryNameClean = string.Empty;
        var keywordNameClean = string.Empty;

        try
        {
            categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());
            keywordNameClean = HttpUtility.HtmlEncode(keywordName.Trim());

            var existingCategory = existingKeywords.Single(keyword => keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

            var updatedCategory = new Keyword
            {
                Name = existingCategory.Name,
                IsCategory = existingCategory.IsCategory,
                Children = existingCategory.Children
                    .Where(keyword => !keyword.Name.Equals(keywordNameClean, StringComparison.Ordinal))
                    .ToList()
            };

            var updatedKeywords = existingKeywords
                .Except(new[] { existingCategory })
                .Concat(new[] { updatedCategory });

            var newKeywords = await _keywordRepository.WriteAsync(updatedKeywords).ConfigureAwait(false);

            _logger.LogInformation("DeleteKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Deleted), categoryNameClean, keywordNameClean);
            return new KeywordResult(ResultType.Deleted, newKeywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Error), categoryNameClean, keywordNameClean);
            return new KeywordResult(ResultType.Error, existingKeywords);
        }
    }

    public async Task<KeywordResult> EditCategoryAsync(string? oldCategoryName, string? newCategoryName)
    {
        if (string.IsNullOrWhiteSpace(oldCategoryName))
            throw new ArgumentNullException(nameof(oldCategoryName));
        if (string.IsNullOrWhiteSpace(newCategoryName))
            throw new ArgumentNullException(nameof(newCategoryName));

        var existingKeywords = (await GetKeywordsAsync().ConfigureAwait(false)).ToArray();

        var oldCategoryNameClean = string.Empty;
        var newCategoryNameClean = string.Empty;

        try
        {
            oldCategoryNameClean = HttpUtility.HtmlEncode(oldCategoryName.Trim());
            newCategoryNameClean = HttpUtility.HtmlEncode(newCategoryName.Trim());

            if (existingKeywords.Any(keyword => keyword.Name.Equals(newCategoryNameClean, StringComparison.Ordinal)))
            {
                _logger.LogWarning("EditCategoryAsync: result={Result} oldCategoryNameClean={OldCategoryNameClean} newCategoryNameClean={NewKeywordNameClean}", nameof(ResultType.Duplicate), oldCategoryNameClean, newCategoryNameClean);
                return new KeywordResult(ResultType.Duplicate, existingKeywords);
            }

            var existingCategory = existingKeywords.Single(keyword => keyword.Name.Equals(oldCategoryNameClean, StringComparison.Ordinal));

            var updatedCategory = new Keyword
            {
                Name = newCategoryNameClean,
                IsCategory = existingCategory.IsCategory,
                Children = existingCategory.Children
            };

            var updatedKeywords = existingKeywords
                .Except(new[] { existingCategory })
                .Concat(new[] { updatedCategory });

            var newKeywords = await _keywordRepository.WriteAsync(updatedKeywords).ConfigureAwait(false);

            _logger.LogInformation("EditCategoryAsync: result={Result} oldCategoryNameClean={OldCategoryNameClean} newCategoryNameClean={NewCategoryNameClean}", nameof(ResultType.Edited), oldCategoryNameClean, newCategoryNameClean);
            return new KeywordResult(ResultType.Edited, newKeywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditCategoryAsync: result={Result} oldCategoryNameClean={OldCategoryNameClean} newCategoryNameClean={NewCategoryNameClean}", nameof(ResultType.Error), oldCategoryNameClean, newCategoryNameClean);
            return new KeywordResult(ResultType.Error, existingKeywords);
        }
    }

    public async Task<KeywordResult> EditKeywordAsync(string? categoryName, string? oldKeywordName, string? newKeywordName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentNullException(nameof(categoryName));
        if (string.IsNullOrWhiteSpace(oldKeywordName))
            throw new ArgumentNullException(nameof(oldKeywordName));
        if (string.IsNullOrWhiteSpace(newKeywordName))
            throw new ArgumentNullException(nameof(newKeywordName));

        var existingKeywords = (await GetKeywordsAsync().ConfigureAwait(false)).ToArray();

        var categoryNameClean = string.Empty;
        var oldKeywordNameClean = string.Empty;
        var newKeywordNameClean = string.Empty;

        try
        {
            categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());
            oldKeywordNameClean = HttpUtility.HtmlEncode(oldKeywordName.Trim());
            newKeywordNameClean = HttpUtility.HtmlEncode(newKeywordName.Trim());

            var existingCategory = existingKeywords.Single(keyword => keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

            if (existingCategory.Children.Any(keyword => keyword.Name.Equals(newKeywordNameClean, StringComparison.Ordinal))) 
            {
                _logger.LogWarning("EditKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} oldKeywordNameClean={OldKeywordNameClean} newKeywordNameClean={NewKeywordNameClean}", nameof(ResultType.Duplicate), categoryNameClean, oldKeywordNameClean, newKeywordNameClean);
                return new KeywordResult(ResultType.Duplicate, existingKeywords);
            }

            var existingKeyword = existingCategory.Children.Single(keyword => keyword.Name.Equals(oldKeywordNameClean, StringComparison.Ordinal));
            
            var newKeyword = new Keyword
            {
                Name = newKeywordNameClean,
                IsCategory = false,
                Children = new List<Keyword>()
            };

            var updatedCategory = new Keyword
            {
                Name = existingCategory.Name,
                IsCategory = existingCategory.IsCategory,
                Children = existingCategory.Children
                    .Except(new[] { existingKeyword })
                    .Concat(new[] { newKeyword })
                    .ToList()
            };

            var updatedKeywords = existingKeywords
                .Except(new[] { existingCategory })
                .Concat(new[] { updatedCategory });

            var newKeywords = await _keywordRepository.WriteAsync(updatedKeywords).ConfigureAwait(false);

            _logger.LogInformation("EditKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} oldKeywordNameClean={OldKeywordNameClean} newKeywordNameClean={NewKeywordNameClean}", nameof(ResultType.Edited), categoryNameClean, oldKeywordNameClean, newKeywordNameClean);
            return new KeywordResult(ResultType.Edited, newKeywords);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex,"EditKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} oldKeywordNameClean={OldKeywordNameClean} newKeywordNameClean={NewKeywordNameClean}", nameof(ResultType.Error), categoryNameClean, oldKeywordNameClean, newKeywordNameClean);
            return new KeywordResult(ResultType.Error, existingKeywords);
        }
    }

    public async Task<IEnumerable<Keyword>> GetKeywordsAsync()
    {
        try
        {
            var keywords = await _keywordRepository.ReadAsync().ConfigureAwait(false);
            if (keywords == null)
                throw new KeyworderException("The repository returned a null keywords collection");

            _logger.LogInformation("GetKeywordsAsync: result={Result}", nameof(ResultType.Success));
            return await Task.FromResult(keywords);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "GetKeywordsAsync: result={Result}", nameof(ResultType.Error));
            throw; 
        }
    }
}