using Newtonsoft.Json;
using Serilog;
using System.Web;

namespace Keyworder.Data;

public class KeywordService
{
    private static readonly object FileAccess = new();

    private readonly FileInfo _keywordsJsonFile;

    public KeywordService(string keywordsJsonPath)
    {
        if (string.IsNullOrWhiteSpace(keywordsJsonPath))
            throw new ArgumentNullException(nameof(keywordsJsonPath));

        if (!File.Exists(keywordsJsonPath))
            File.Create(keywordsJsonPath);

        _keywordsJsonFile = new FileInfo(keywordsJsonPath);
    }

    public async Task<ResultType> CreateCategoryAsync(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentNullException(nameof(categoryName));

        var existingKeywords = (await GetKeywordsAsync().ConfigureAwait(false))
            .ToArray();

        var categoryNameClean = string.Empty;
        
        try
        {
            categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());

            var existingCategory = existingKeywords.FirstOrDefault(keyword => keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

            if (existingCategory != null)
            {
                Log.Warning("CreateCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Duplicate), categoryNameClean);
                return ResultType.Duplicate;
            }

            var newCategory = new Keyword
            {
                Name = categoryNameClean,
                IsCategory = true,
                Children = new List<Keyword>()
            };

            var keywordsJson = JsonConvert.SerializeObject(existingKeywords.Concat(new[] { newCategory }));
            lock (FileAccess)
            {
                File.WriteAllText(_keywordsJsonFile.FullName, keywordsJson);
            }

            Log.Information("CreateCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Created), categoryNameClean);
            return ResultType.Created;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "CreateCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Error), categoryNameClean);
            return ResultType.Error;
        }
    }

    public async Task<ResultType> CreateKeywordAsync(string? categoryName, string? keywordName)
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
                Log.Warning("CreateKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Duplicate), categoryNameClean, keywordNameClean);
                return ResultType.Duplicate;
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

            var keywordsJson = JsonConvert.SerializeObject(updatedKeywords);
            lock (FileAccess)
            {
                File.WriteAllText(_keywordsJsonFile.FullName, keywordsJson);
            }

            Log.Information("CreateKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Created), categoryNameClean, keywordNameClean);
            return ResultType.Created;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "CreateKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Error), categoryNameClean, keywordNameClean);
            return ResultType.Error;
        }
    }

    public async Task<ResultType> DeleteCategoryAsync(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentNullException(nameof(categoryName));

        var existingKeywords = await GetKeywordsAsync().ConfigureAwait(false);

        var categoryNameClean = string.Empty;

        try
        {
            categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());

            var updatedKeywords = existingKeywords.Where(keyword => !keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

            var keywordsJson = JsonConvert.SerializeObject(updatedKeywords);
            lock (FileAccess)
            {
                File.WriteAllText(_keywordsJsonFile.FullName, keywordsJson);
            }

            Log.Information("DeleteCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Deleted), categoryNameClean);
            return ResultType.Deleted;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DeleteCategoryAsync: result={Result} categoryNameClean={CategoryNameClean}", nameof(ResultType.Error), categoryNameClean);
            return ResultType.Error;
        }
    }

    public async Task<ResultType> DeleteKeywordAsync(string? categoryName, string? keywordName)
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

            var keywordsJson = JsonConvert.SerializeObject(updatedKeywords);
            lock (FileAccess)
            {
                File.WriteAllText(_keywordsJsonFile.FullName, keywordsJson);
            }

            Log.Information("DeleteKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Deleted), categoryNameClean, keywordNameClean);
            return ResultType.Deleted;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DeleteKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} keywordNameClean={KeywordNameClean}", nameof(ResultType.Error), categoryNameClean, keywordNameClean);
            return ResultType.Error;
        }
    }

    public async Task<ResultType> EditCategoryAsync(string? oldCategoryName, string? newCategoryName)
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
                Log.Warning("EditCategoryAsync: result={Result} oldCategoryNameClean={OldCategoryNameClean} newCategoryNameClean={NewKeywordNameClean}", nameof(ResultType.Duplicate), oldCategoryNameClean, newCategoryNameClean);
                return ResultType.Duplicate;
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

            var keywordsJson = JsonConvert.SerializeObject(updatedKeywords);
            lock (FileAccess)
            {
                File.WriteAllText(_keywordsJsonFile.FullName, keywordsJson);
            }

            Log.Information("EditCategoryAsync: result={Result} oldCategoryNameClean={OldCategoryNameClean} newCategoryNameClean={NewCategoryNameClean}", nameof(ResultType.Edited), oldCategoryNameClean, newCategoryNameClean);
            return ResultType.Edited;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "EditCategoryAsync: result={Result} oldCategoryNameClean={OldCategoryNameClean} newCategoryNameClean={NewCategoryNameClean}", nameof(ResultType.Error), oldCategoryNameClean, newCategoryNameClean);
            return ResultType.Error;
        }
    }

    public async Task<ResultType> EditKeywordAsync(string? categoryName, string? oldKeywordName, string? newKeywordName)
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
                Log.Warning("EditKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} oldKeywordNameClean={OldKeywordNameClean} newKeywordNameClean={NewKeywordNameClean}", nameof(ResultType.Duplicate), categoryNameClean, oldKeywordNameClean, newKeywordNameClean);
                return ResultType.Duplicate;
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

            var keywordsJson = JsonConvert.SerializeObject(updatedKeywords);
            lock (FileAccess)
            {
                File.WriteAllText(_keywordsJsonFile.FullName, keywordsJson);
            }

            Log.Information("EditKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} oldKeywordNameClean={OldKeywordNameClean} newKeywordNameClean={NewKeywordNameClean}", nameof(ResultType.Edited), categoryNameClean, oldKeywordNameClean, newKeywordNameClean);
            return ResultType.Edited;
        }
        catch (Exception ex)
        {
            Log.Information(ex,"EditKeywordAsync: result={Result} categoryNameClean={CategoryNameClean} oldKeywordNameClean={OldKeywordNameClean} newKeywordNameClean={NewKeywordNameClean}", nameof(ResultType.Error), categoryNameClean, oldKeywordNameClean, newKeywordNameClean);
            return ResultType.Error;
        }
    }

    public async Task<IEnumerable<Keyword>> GetKeywordsAsync()
    {
        try
        {
            string keywordsJson;
            lock (FileAccess)
            {
                keywordsJson = File.ReadAllText(_keywordsJsonFile.FullName);
            }

            var keywords = JsonConvert.DeserializeObject<IEnumerable<Keyword>>(keywordsJson);
            if (keywords == null)
                throw new KeyworderException("The specified json path deserialized to a null keywords collection");

            Log.Information("GetKeywordsAsync: result={Result}", nameof(ResultType.Success));

            return await Task.FromResult(keywords);
        }
        catch (Exception ex)
        {
            Log.Information(ex, "GetKeywordsAsync: result={Result}", nameof(ResultType.Error));
            throw; 
        }
    }
}
