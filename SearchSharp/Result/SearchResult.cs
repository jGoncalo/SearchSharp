using SearchSharp.Engine;

namespace SearchSharp.Result;

public class SearchResult<TQueryData> : ISearchResult<TQueryData>
    where TQueryData : QueryData {
    public ISearchInput Input { get; init; } = new SearchInput();
    public int Total { get; init; }

    public TQueryData[] Content { get; init; } = Array.Empty<TQueryData>();
}

public class SearchResult : SearchResult<QueryData>, ISearchResult {
    
}