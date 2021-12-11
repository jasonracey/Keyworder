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
                    Keywords = new List<Keyword>()
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

                if (existingCategory.Keywords.Any(keyword => keyword.Name.Equals(keywordNameClean, StringComparison.Ordinal)))
                    return ResultType.Duplicate;

                var updatedCategory = new Keyword
                {
                    Name = existingCategory.Name,
                    Keywords = existingCategory.Keywords
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
                    Keywords = existingCategory.Keywords
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
