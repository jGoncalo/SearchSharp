using SearchSharp.Engine;

namespace SearchSharp.Result;

/// <summary>
/// Result of an executed query
/// </summary>
/// <typeparam name="TQueryData">DataType of the result contents</typeparam>
public interface ISearchResult<TQueryData> 
    where TQueryData : QueryData {
    /// <summary>
    /// Input information
    /// </summary>
    public ISearchInput Input { get; }
    /// <summary>
    /// Total results in repostiory
    /// regardless of applied query
    /// </summary>
    public int Total { get; }

    /// <summary>
    /// Results found that matched evaluated query
    /// </summary>
    public TQueryData[] Content { get; }
}

/// <summary>
/// Generic representation of a search result
/// Content is cast as generic QueryData
/// </summary>
public interface ISearchResult : ISearchResult<QueryData> {

}