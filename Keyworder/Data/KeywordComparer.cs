using System.Diagnostics.CodeAnalysis;

namespace Keyworder.Data
{
    public class KeywordComparer : IEqualityComparer<Keyword>
    {
        private static readonly IEnumerable<Keyword> Empty = Enumerable.Empty<Keyword>();

        public bool Equals(Keyword? k1, Keyword? k2)
        {
            return 
                k1?.Name == k2?.Name && 
                Enumerable.SequenceEqual(k1?.Children ?? Empty, k2?.Children ?? Empty);
        }

        public int GetHashCode([DisallowNull] Keyword k)
        {
            var nameHashCode = k?.Name != null 
                ? k.Name.GetHashCode()
                : 0;

            int keywordsHashCode = 0;
            if (k?.Children != null)
            {
                foreach (var keyword in k?.Children ?? Empty)
                {
                    keywordsHashCode ^= keyword.GetHashCode();
                }
            }

            return nameHashCode ^ keywordsHashCode;
        }
    }
}
