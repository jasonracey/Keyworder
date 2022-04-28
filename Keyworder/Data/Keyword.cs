namespace Keyworder.Data;

public record Keyword
{
    private readonly string _name = string.Empty;

    public string Name
    {
        get => _name;
        init => _name = value ?? throw new ArgumentNullException(nameof(value)); 
    }

    public bool IsCategory { get; init; }

    public List<Keyword> Children { get; init; } = new();
}
