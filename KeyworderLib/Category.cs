using System;
using System.Collections.Generic;

namespace KeyworderLib
{
    public class Category
    {
        public string CategoryId { get; private set; }

        public SortedSet<Keyword> Keywords { get; private set; }

        public Category(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new ArgumentNullException(nameof(categoryId));

            CategoryId = categoryId;
            Keywords = new SortedSet<Keyword>(new KeywordComparer());
        }
    }
}
