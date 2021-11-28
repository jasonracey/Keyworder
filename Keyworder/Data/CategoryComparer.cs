namespace Keyworder.Data;

public class CategoryComparer : IComparer<Category>
{
    public int Compare(Category? x, Category? y)
    {
        return string.Compare(x?.CategoryId, y?.CategoryId, StringComparison.Ordinal);
    }
}