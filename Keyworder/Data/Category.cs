namespace Keyworder.Data;

public class Category
{
    private static readonly KeywordComparer keywordComparer = new();

    public string CategoryId { get; private set; }

    public ICollection<Keyword> Keywords { get; private set; }

    public Category(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentNullException(nameof(categoryId));
            
        CategoryId = categoryId;
        Keywords = new SortedSet<Keyword>(keywordComparer);
    }
}
