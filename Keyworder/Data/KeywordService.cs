using Newtonsoft.Json;
using System.Web;

namespace Keyworder.Data
{
    public class KeywordService
    {
        private readonly ReaderWriterLockSlim fileLock = new();

        private readonly FileInfo keywordsJsonFile;

        public KeywordService(string keywordsJsonPath)
        {
            if (string.IsNullOrWhiteSpace(keywordsJsonPath))
                throw new ArgumentNullException(nameof(keywordsJsonPath));

            if (!File.Exists(keywordsJsonPath))
                File.Create(keywordsJsonPath);

            keywordsJsonFile = new FileInfo(keywordsJsonPath);
        }

        public async Task<ResultType> CreateCategoryAsync(string? categoryName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));

            var existingKeywords = await GetKeywordsAsync().ConfigureAwait(false);

            fileLock.EnterWriteLock();

            try
            {
                var categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());

                var existingCategory = existingKeywords.FirstOrDefault(keyword => keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

                if (existingCategory != null)
                    return ResultType.Duplicate;

                var newCategory = new Keyword
                {
                    Name = categoryNameClean,
                    IsCategory = true,
                    Children = new List<Keyword>()
                };

                var keywordsJson = JsonConvert.SerializeObject(existingKeywords.Concat(new[] { newCategory }));
                File.WriteAllText(this.keywordsJsonFile.FullName, keywordsJson);

                return ResultType.Success;
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
        }

        public async Task<ResultType> CreateKeywordAsync(string? categoryName, string? keywordName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));
            if (keywordName == null)
                throw new ArgumentNullException(nameof(keywordName));

            var existingKeywords = await GetKeywordsAsync().ConfigureAwait(false);

            fileLock.EnterWriteLock();

            try
            {
                var categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());
                var keywordNameClean = HttpUtility.HtmlEncode(keywordName.Trim());

                var existingCategory = existingKeywords.Single(keyword => keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

                if (existingCategory.Children.Any(keyword => keyword.Name.Equals(keywordNameClean, StringComparison.Ordinal)))
                    return ResultType.Duplicate;

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
                File.WriteAllText(this.keywordsJsonFile.FullName, keywordsJson);

                return ResultType.Success;
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
        }

        public async Task<ResultType> DeleteCategoryAsync(string? categoryName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));

            var existingKeywords = await GetKeywordsAsync().ConfigureAwait(false);

            fileLock.EnterWriteLock();

            try
            {
                var categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());

                var updatedKeywords = existingKeywords.Where(keyword => !keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

                var keywordsJson = JsonConvert.SerializeObject(updatedKeywords);
                File.WriteAllText(this.keywordsJsonFile.FullName, keywordsJson);

                return ResultType.Success;
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
        }

        public async Task<ResultType> DeleteKeywordAsync(string? categoryName, string? keywordName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));
            if (keywordName == null)
                throw new ArgumentNullException(nameof(keywordName));

            var existingKeywords = await GetKeywordsAsync().ConfigureAwait(false);

            fileLock.EnterWriteLock();

            try
            {
                var categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());
                var keywordNameClean = HttpUtility.HtmlEncode(keywordName.Trim());

                var existingCategory = existingKeywords.Single(keyword => keyword.Name.Equals(categoryName, StringComparison.Ordinal));

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
                File.WriteAllText(this.keywordsJsonFile.FullName, keywordsJson);

                return ResultType.Success;
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
        }

        public async Task<ResultType> EditCategoryAsync(string? oldCategoryName, string? newCategoryName)
        {
            if (oldCategoryName == null)
                throw new ArgumentNullException(nameof(oldCategoryName));
            if (newCategoryName == null)
                throw new ArgumentNullException(nameof(newCategoryName));

            var existingKeywords = await GetKeywordsAsync().ConfigureAwait(false);

            fileLock.EnterWriteLock();

            try
            {
                var oldCategoryNameClean = HttpUtility.HtmlEncode(oldCategoryName.Trim());
                var newCategoryNameClean = HttpUtility.HtmlEncode(newCategoryName.Trim());

                var existingCategory = existingKeywords.Single(keyword => keyword.Name.Equals(oldCategoryNameClean, StringComparison.Ordinal));

                if (existingCategory.Children.Any(keyword => keyword.Name.Equals(newCategoryNameClean, StringComparison.Ordinal)))
                    return ResultType.Duplicate;

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
                File.WriteAllText(this.keywordsJsonFile.FullName, keywordsJson);

                return ResultType.Success;
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
        }

        public async Task<ResultType> EditKeywordAsync(string? categoryName, string? oldKeywordName, string? newKeywordName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));
            if (oldKeywordName == null)
                throw new ArgumentNullException(nameof(oldKeywordName));
            if (newKeywordName == null)
                throw new ArgumentNullException(nameof(newKeywordName));

            var existingKeywords = await GetKeywordsAsync().ConfigureAwait(false);

            fileLock.EnterWriteLock();

            try
            {
                var categoryNameClean = HttpUtility.HtmlEncode(categoryName.Trim());
                var oldKeywordNameClean = HttpUtility.HtmlEncode(oldKeywordName.Trim());
                var newKeywordNameClean = HttpUtility.HtmlEncode(newKeywordName.Trim());

                var existingCategory = existingKeywords.Single(keyword => keyword.Name.Equals(categoryNameClean, StringComparison.Ordinal));

                if (existingCategory.Children.Any(keyword => keyword.Name.Equals(newKeywordNameClean, StringComparison.Ordinal)))
                    return ResultType.Duplicate;

                var existingKeyword = existingCategory.Children.Single(Keyword => Keyword.Name.Equals(oldKeywordNameClean, StringComparison.Ordinal));
                
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
                File.WriteAllText(this.keywordsJsonFile.FullName, keywordsJson);

                return ResultType.Success;
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
        }

        public async Task<IEnumerable<Keyword>> GetKeywordsAsync()
        {
            fileLock.EnterReadLock();

            try
            {
                var keywordsJson = File.ReadAllText(this.keywordsJsonFile.FullName);

                var keywords = JsonConvert.DeserializeObject<IEnumerable<Keyword>>(keywordsJson);
                if (keywords == null)
                    throw new KeyworderException("The specified json path deserialized to a null keywords collection");

                return await Task.FromResult(keywords)
                    .ConfigureAwait(false);
            }
            finally
            {
                fileLock.ExitReadLock();
            }
        }

        ~KeywordService()
        {
            if (fileLock != null) 
                fileLock.Dispose();
        }
    }
}
