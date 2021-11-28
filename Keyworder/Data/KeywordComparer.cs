namespace Keyworder.Data;

public class KeywordComparer : IComparer<Keyword>
{
    public int Compare(Keyword? x, Keyword? y)
    {
        return string.Compare(x?.KeywordId, y?.KeywordId, StringComparison.Ordinal);
    }
}