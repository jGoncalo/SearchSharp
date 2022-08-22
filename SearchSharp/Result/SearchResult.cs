using SearchSharp.Engine;

namespace SearchSharp.Result;

/// <summary>
/// Result of an executed query
/// </summary>
/// <typeparam name="TQueryData">DataType of the result contents</typeparam>
public class SearchResult<TQueryData> : ISearchResult<TQueryData>
    where TQueryData : QueryData {
    /// <summary>
    /// Input information
    /// </summary>
    public ISearchInput Input { get; init; } = new SearchInput();
    /// <summary>
    /// Total results in repostiory
    /// regardless of applied query
    /// </summary>
    public int Total { get; init; }

    /// <summary>
    /// Results found that matched evaluated query
    /// </summary>
    public TQueryData[] Content { get; init; } = Array.Empty<TQueryData>();
}

/// <summary>
/// Generic representation of a search result
/// Content is cast as generic QueryData
/// </summary>
public class SearchResult : SearchResult<QueryData>, ISearchResult {
    
}