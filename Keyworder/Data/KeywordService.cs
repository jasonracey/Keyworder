using Newtonsoft.Json;

namespace Keyworder.Data
{
    public class KeywordService
    {
        private readonly FileInfo keywordsJsonFile;

        public KeywordService(string keywordsJsonPath)
        {
            if (string.IsNullOrWhiteSpace(keywordsJsonPath))
                throw new ArgumentNullException(nameof(keywordsJsonPath));

            if (!File.Exists(keywordsJsonPath))
                File.Create(keywordsJsonPath);

            keywordsJsonFile = new FileInfo(keywordsJsonPath);
        }

        public async Task<IEnumerable<Keyword>> GetKeywordsAsync()
        {
            var keywordsJson = File.ReadAllText(this.keywordsJsonFile.FullName);

            var keywords = JsonConvert.DeserializeObject<IEnumerable<Keyword>>(keywordsJson);
            if (keywords == null)
                throw new KeyworderException("The specified json path deserialized to a null keywords collection");

            return await Task.FromResult(keywords).ConfigureAwait(false);
        }
    }
}
