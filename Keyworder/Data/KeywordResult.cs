namespace Keyworder.Data;

public record KeywordResult(ResultType ResultType, IEnumerable<Keyword> Keywords);