using SearchSharp.Engine;
using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Exceptions;
using Sprache;

namespace SearchSharp.Domain;

public class SearchDomain : ISearchDomain
{
    public class Builder {
        private readonly Dictionary<string, ISearchEngine> _engines = new();
        private string _defaultAlias = string.Empty;

        public Builder Add(ISearchEngine engine) {
            _engines[engine.Alias] = engine;
            if(_engines.Count() == 1) _defaultAlias = engine.Alias;
            return this;
        }

        public Builder With<TQueryData>(string alias, Action<SearchEngine<TQueryData>.Builder> configuration) where TQueryData : QueryData {
            var builder = new SearchEngine<TQueryData>.Builder(alias);
            configuration(builder);

            Add(builder.Build());
            return this;
        }

        public Builder SetDefaultEngine(string alias) {
            if(_engines.ContainsKey(alias)) _defaultAlias = alias;
            return this;
        }

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

    public ISearchEngine this[string alias] => _engines[alias];
    public bool TryGet(string alias, out ISearchEngine? engine)
    {
        return _engines.TryGetValue(alias, out engine);
    }
    public bool TryGet<TQueryData>(string alias, out ISearchEngine<TQueryData>? engine) where TQueryData : QueryData {
        var found = _engines.TryGetValue(alias, out var genericEngine);
        
        if(found && genericEngine!.DataType == typeof(TQueryData)) {
            engine = (genericEngine as ISearchEngine<TQueryData>)!;
            return true;
        }
        engine = null;
        return false;
    }

    public ISearchResult Search(string query, string? engineAlias = null, string? dataProvider = null) {
        var parseResult = QueryParser.Query.TryParse(query);
        if(!parseResult.WasSuccessful) throw new SearchExpception(parseResult.Message);
        var parsedQuery = parseResult.Value;

        return Search(parsedQuery, engineAlias, dataProvider);
    }
    public ISearchResult Search(Query query, string? engineAlias = null, string? dataProvider = null) {
        var targetAlias = engineAlias ?? query.Provider?.EngineAlias ?? _defaultEngineAlias;
        var hasEngine = TryGet(targetAlias, out var engine);
        
        if(!hasEngine) throw new SearchExpception("");

        var result = engine!.Query(query, dataProvider);
        return new SearchResult {
            Input = result.Input,
            Total = result.Total,
            Content = result.Content
        };
    }
    public Task<ISearchResult> SearchAsync(string query, string? engineAlias = null, string? dataProvider = null) {
        return Task.FromResult(Search(query, engineAlias, dataProvider));
    }
    public Task<ISearchResult> SearchAsync(Query query, string? engineAlias = null, string? dataProvider = null) {
        return Task.FromResult(Search(query, engineAlias, dataProvider));
    }

    public ISearchResult<TQueryData> Search<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData
    {
        var parseResult = QueryParser.Query.TryParse(query);
        if(!parseResult.WasSuccessful) throw new SearchExpception(parseResult.Message);

        var parsed = QueryParser.Query.TryParse(query);
        if(!parsed.WasSuccessful) throw new SearchExpception("");
        var parsedQuery = parsed.Value;

        return Search<TQueryData>(parsedQuery, engineAlias, dataProvider);
    }
    public ISearchResult<TQueryData> Search<TQueryData>(Query query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData
    {
        var targetAlias = engineAlias ?? query.Provider?.EngineAlias ?? _defaultEngineAlias;
        var hasEngine = TryGet<TQueryData>(targetAlias, out var engine);
        
        if(!hasEngine) throw new SearchExpception("");

        return engine!.Query(query, dataProvider);
    }
    public Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData
    {
        return Task.FromResult(Search<TQueryData>(query, engineAlias, dataProvider));
    }
    public Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(Query query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData
    {
        return Task.FromResult(Search<TQueryData>(query, engineAlias, dataProvider));
    }
}