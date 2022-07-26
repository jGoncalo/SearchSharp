namespace SearchSharp.Domain;

public interface ISearchInput {
    public string Query { get; }
    public string EvaluatedExpression { get; }
    public string[] Commands { get; }
}

public interface ISearchResult<TQueryData> 
    where TQueryData : class {
    public ISearchInput Input { get; }
    public int Total { get; }

    public TQueryData[] Content { get; }
}

public class SearchInput : ISearchInput {
    public string Query { get; init; } = string.Empty;
    public string EvaluatedExpression { get; init; } = string.Empty;
    public string[] Commands { get; init; } = Array.Empty<string>();
}

public class SearchResult<TQueryData> : ISearchResult<TQueryData>
    where TQueryData : class {
    public ISearchInput Input { get; init; } = new SearchInput();
    public int Total { get; init; }

    public TQueryData[] Content { get; init; } = Array.Empty<TQueryData>();
}