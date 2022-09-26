using SearchSharp.Engine;
using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Exceptions;
using SearchSharp.Result;
using Sprache;

namespace SearchSharp.Domain;

/// <summary>
/// Search domain, containing multiple search engines
/// Allows for engine agnostic search
/// </summary>
public class SearchDomain : ISearchDomain
{
    /// <summary>
    /// Search Domain Builder
    /// </summary>
    public class Builder {
        private readonly Dictionary<string, ISearchEngine> _engines = new();
        private string _defaultAlias = string.Empty;

        /// <summary>
        /// Register a search engine
        /// </summary>
        /// <param name="engine">Search engine</param>
        /// <returns>This builder</returns>
        public Builder Add(ISearchEngine engine) {
            _engines[engine.Alias] = engine;
            if(_engines.Count() == 1) _defaultAlias = engine.Alias;
            return this;
        }

        /// <summary>
        /// Register a search engine
        /// </summary>
        /// <typeparam name="TQueryData">Data type associated with engine</typeparam>
        /// <param name="alias">Engine unique alias</param>
        /// <param name="configuration">Action configure search engine via builder</param>
        /// <returns></returns>
        public Builder With<TQueryData>(string alias, Action<SearchEngine<TQueryData>.Builder> configuration) where TQueryData : QueryData {
            var builder = new SearchEngine<TQueryData>.Builder(alias);
            configuration(builder);

            Add(builder.Build());
            return this;
        }

        /// <summary>
        /// Set default search engine
        /// </summary>
        /// <param name="alias">Unique search engine</param>
        /// <returns>This builder</returns>
        public Builder SetDefaultEngine(string alias) {
            if(_engines.ContainsKey(alias)) _defaultAlias = alias;
            return this;
        }

        /// <summary>
        /// Build Search Domain
        /// </summary>
        /// <returns>Search domain</returns>
        public ISearchDomain Build(){
            return new SearchDomain(_defaultAlias, _engines);
        }
    }

    private readonly IReadOnlyDictionary<string, ISearchEngine> _engines;
    private readonly string _defaultEngineAlias;

    private SearchDomain(string defaultEngineAlias, IReadOnlyDictionary<string, ISearchEngine> engines)
    {
        _defaultEngineAlias = defaultEngineAlias;
        _engines = engines;
    }

    /// <summary>
    /// Obtain a specific search engine
    /// </summary>
    /// <param name="alias">Search engine alias</param>
    /// <returns>The found search engine</returns>
    public ISearchEngine this[string alias] => _engines[alias];
    /// <summary>
    /// Try to obtain a specific search engine
    /// </summary>
    /// <param name="alias">Search engine alias</param>
    /// <param name="engine">found engine (null if none found)</param>
    /// <returns>If an engine with the given alias was found</returns>
    public bool TryGet(string alias, out ISearchEngine? engine)
    {
        return _engines.TryGetValue(alias, out engine);
    }
    /// <summary>
    /// Try to obtain a specific search engine
    /// </summary>
    /// <typeparam name="TQueryData">Engine data type</typeparam>
    /// <param name="alias">Search engine alias</param>
    /// <param name="engine">found engine (null if none found)</param>
    /// <returns>If an engine with the given alias was found, and matching data type</returns>
    public bool TryGet<TQueryData>(string alias, out ISearchEngine<TQueryData>? engine) where TQueryData : QueryData {
        var found = _engines.TryGetValue(alias, out var genericEngine);
        
        if(found && genericEngine!.DataType == typeof(TQueryData)) {
            engine = (genericEngine as ISearchEngine<TQueryData>)!;
            return true;
        }
        engine = null;
        return false;
    }

    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <returns>Search results</returns>
    public ISearchResult Search(string query, string? engineAlias = null, string? dataProvider = null) {
        return SearchAsync(query, engineAlias, dataProvider).Await();
    }
    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query record</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <returns>Search results</returns>
    public ISearchResult Search(Query query, string? engineAlias = null, string? dataProvider = null) {
        return SearchAsync(query, engineAlias, dataProvider).Await();
    }
    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <param name="ct">Task cancellation token</param>
    /// <returns>Task to obtain search results</returns>
    public Task<ISearchResult> SearchAsync(string query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default) {
        var parseResult = QueryParser.Query.TryParse(query);
        if(!parseResult.WasSuccessful) throw new SearchExpception(parseResult.Message);
        var parsedQuery = parseResult.Value;

        return SearchAsync(parsedQuery, engineAlias, dataProvider, ct);
    }
    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query record</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <param name="ct">Task cancellation token</param>
    /// <returns>Task to obtain search results</returns>
    public async Task<ISearchResult> SearchAsync(Query query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default) {
        var targetAlias = engineAlias ?? query.Provider?.EngineAlias ?? _defaultEngineAlias;
        var hasEngine = TryGet(targetAlias, out var engine);
        
        if(!hasEngine) throw new SearchExpception("TODO");

        return await engine!.QueryAsync(query, dataProvider, ct);
    }

    /// <summary>
    /// Search for results
    /// </summary>
    /// <param name="query">query string</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <returns>Search results</returns>
    public ISearchResult<TQueryData> Search<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData
    {
        return SearchAsync<TQueryData>(query, engineAlias, dataProvider).Await();
    }
    /// <summary>
    /// Search for results
    /// </summary>
    /// <typeparam name="TQueryData">Engine data type</typeparam>
    /// <param name="query">query record</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <returns>Search results</returns>
    public ISearchResult<TQueryData> Search<TQueryData>(Query query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData
    {
        return SearchAsync<TQueryData>(query, engineAlias, dataProvider).Await();
    }
    /// <summary>
    /// Search for results
    /// </summary>
    /// <typeparam name="TQueryData">Engine data type</typeparam>
    /// <param name="query">query string</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <param name="ct">Task cancellation token</param>
    /// <returns>Task to obtain search results</returns>
    public Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default) where TQueryData : QueryData
    {
        var parseResult = QueryParser.Query.TryParse(query);
        if(!parseResult.WasSuccessful) throw new SearchExpception(parseResult.Message);
        var parsedQuery = parseResult.Value;

        return SearchAsync<TQueryData>(parsedQuery, engineAlias, dataProvider, ct);
    }
    /// <summary>
    /// Search for results
    /// </summary>
    /// <typeparam name="TQueryData">Engine data type</typeparam>
    /// <param name="query">query record</param>
    /// <param name="engineAlias">target engine alias</param>
    /// <param name="dataProvider">engine data provider</param>
    /// <param name="ct">Task cancellation token</param>
    /// <returns>Task to obtain search results</returns>
    public async Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(Query query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default) where TQueryData : QueryData
    {
        var targetAlias = engineAlias ?? query.Provider?.EngineAlias ?? _defaultEngineAlias;
        var hasEngine = TryGet<TQueryData>(targetAlias, out var engine);
        
        if(!hasEngine) throw new SearchExpception("TODO");

        return await engine!.QueryAsync(query, dataProvider, ct);
    }
}