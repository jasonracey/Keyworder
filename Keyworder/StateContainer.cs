using Keyworder.Data;

namespace Keyworder;

public class StateContainer
{
    private List<Keyword> _selectedKeywords = new();
    
    public List<Keyword> SelectedKeywords
    {
        get => _selectedKeywords;
        set => _selectedKeywords = value ?? throw new ArgumentNullException(nameof(value));
    }
}