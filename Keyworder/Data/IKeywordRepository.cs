namespace Keyworder.Data;

public interface IKeywordRepository
{
    Task<IEnumerable<Keyword>> ReadAsync();
    Task<IEnumerable<Keyword>> WriteAsync(IEnumerable<Keyword> keywords);
}