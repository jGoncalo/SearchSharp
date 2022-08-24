using SearchSharp.Result;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine;

/// <summary>
/// Obtain targeted results from a given repository, based on a query string
/// </summary>
public interface ISearchEngine {
    /// <summary>
    /// Type of data returned by queries
    /// </summary>
    public Type DataType { get; }
    /// <summary>
    /// Engine alias, used to uniquely identify an engine
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Obtain data when applying a query constraint to a repository
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="dataProvider">targeted data provider</param>
    /// <returns>Matching data</returns>
    ISearchResult Query(string query, string? dataProvider = null);
    /// <summary>
    /// Obtain data when applying a query constraint to a repository
    /// </summary>
    /// <param name="query">query record</param>
    /// <param name="dataProvider">targeted data provider</param>
    /// <returns>Matching data</returns>
    ISearchResult Query(Query query, string? dataProvider = null);

    /// <summary>
    /// Obtain data when applying a query constraint to a repository
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="dataProvider">targeted data provider</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching data</returns>
    Task<ISearchResult> QueryAsync(string query, string? dataProvider = null, CancellationToken ct = default);
    /// <summary>
    /// Obtain data when applying a query constraint to a repository
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="dataProvider">targeted data provider</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching data</returns>
    Task<ISearchResult> QueryAsync(Query query, string? dataProvider = null, CancellationToken ct = default);
}

/// <summary>
/// Obtain targeted results from a given repository, based on a query string
/// </summary>
/// <typeparam name="TQueryData">Data type of results</typeparam>
public interface ISearchEngine<TQueryData> : ISearchEngine where TQueryData : QueryData {
    /// <summary>
    /// Obtain data when applying a query constraint to a repository
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="dataProvider">targeted data provider</param>
    /// <returns>Matching data</returns>
    new ISearchResult<TQueryData> Query(string query, string? dataProvider = null);
    /// <summary>
    /// Obtain data when applying a query constraint to a repository
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="dataProvider">targeted data provider</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching data</returns>
    new Task<ISearchResult<TQueryData>> QueryAsync(string query, string? dataProvider = null, CancellationToken ct = default);
    
    /// <summary>
    /// Obtain data when applying a query constraint to a repository
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="dataProvider">targeted data provider</param>
    /// <returns>Matching data</returns>
    new ISearchResult<TQueryData> Query(Query query, string? dataProvider = null);
    /// <summary>
    /// Obtain data when applying a query constraint to a repository
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="dataProvider">targeted data provider</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Matching data</returns>
    new Task<ISearchResult<TQueryData>> QueryAsync(Query query, string? dataProvider = null, CancellationToken ct = default);
}
