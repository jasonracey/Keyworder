using System;

namespace KeyworderLib
{
    public class Keyword
    {
        public string CategoryId { get; private set; }

        public string KeywordId { get; private set; }

        public Keyword(string categoryId, string keywordId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentNullException(nameof(categoryId));
            if (string.IsNullOrWhiteSpace(keywordId))
                throw new ArgumentNullException(nameof(keywordId));

            CategoryId = categoryId;
            KeywordId = keywordId;
        }
    }
}
