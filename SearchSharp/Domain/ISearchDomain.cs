using SearchSharp.Result;
using SearchSharp.Engine;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Domain;

/// <summary>
/// Search domain, containing multiple search engines
/// Allows for engine agnostic search
/// </summary>
public interface ISearchDomain {
    /// <summary>
    /// Obtain a specific search engine
    /// </summary>
    /// <param name="alias">Search engine alias</param>
    /// <returns>The found search engine</returns>
    ISearchEngine this[string alias] { get; }

    /// <summary>
    /// Try to obtain a specific search engine
    /// </summary>
    /// <param name="alias">Search engine alias</param>
    /// <param name="engine">found engine (null if none found)</param>
    /// <returns>If an engine with the given alias was found</returns>
    bool TryGet(string alias, out ISearchEngine? engine);
    /// <summary>
    /// Try to obtain a specific search engine
    /// </summary>
    /// <typeparam name="TQueryData">Engine data type</typeparam>
    /// <param name="alias">Search engine alias</param>
    /// <param name="engine">found engine (null if none found)</param>
    /// <returns>If an engine with the given alias was found, and matching data type</returns>
    bool TryGet<TQueryData>(string alias, out ISearchEngine<TQueryData>? engine) where TQueryData : QueryData;

    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <returns>Search results</returns>
    ISearchResult Search(string query, string? engineAlias = null, string? dataProvider = null);
    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query record</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <returns>Search results</returns>
    ISearchResult Search(Query query, string? engineAlias = null, string? dataProvider = null);
    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <param name="ct">Task cancellation token</param>
    /// <returns>Task to obtain search results</returns>
    Task<ISearchResult> SearchAsync(string query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default);
    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query record</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <param name="ct">Task cancellation token</param>
    /// <returns>Task to obtain search results</returns>
    Task<ISearchResult> SearchAsync(Query query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default);

    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <returns>Search results</returns>
    ISearchResult<TQueryData> Search<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData;
    /// <summary>
    /// Search for results
    /// </summary>
    /// <typeparam name="TQueryData">Engine data type</typeparam>
    /// <param name="query">query record</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <returns>Search results</returns>
    ISearchResult<TQueryData> Search<TQueryData>(Query query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData;
    /// <summary>
    /// Search for results
    /// </summary>
    /// <typeparam name="TQueryData">Engine data type</typeparam>
    /// <param name="query">query string</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <param name="ct">Task cancellation token</param>
    /// <returns>Task to obtain search results</returns>
    Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default) where TQueryData : QueryData;
    /// <summary>
    /// Search for results
    /// </summary>
    /// <typeparam name="TQueryData">Engine data type</typeparam>
    /// <param name="query">query record</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <param name="ct">Task cancellation token</param>
    /// <returns>Task to obtain search results</returns>
    Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(Query query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default) where TQueryData : QueryData;
}