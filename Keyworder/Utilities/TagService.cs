namespace Keyworder.Utilities;

public static class TagService
{
    public static string ToFlickrTagsString(IEnumerable<string> values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        var quotedOrderedValues = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .Select(value => $"\"{value}\"");

        return string.Join(",", quotedOrderedValues);
    }
}