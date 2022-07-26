using SearchSharp.Engine;
using SearchSharp.Engine.Config;
using SearchSharp.Exceptions;

namespace SearchSharp.Domain;

public class SearchDomain : ISearchDomain
{
    public class Builder {
        private readonly Dictionary<Type, ISearchEngine> _engines = new();

        public Builder Add(ISearchEngine engine) {
            _engines[engine.DataType] = engine;
            return this;
        }

        public Builder With<TQueryData>(Action<SearchEngine<TQueryData>.Builder> configuration) where TQueryData : class {
            var builder = new SearchEngine<TQueryData>.Builder();
            configuration(builder);

            _engines[typeof(TQueryData)] = builder.Build();
            return this;
        }

        public ISearchDomain Build(){
            return new SearchDomain(_engines);
        }
    }

    private readonly IReadOnlyDictionary<Type, ISearchEngine> _engines;

    public SearchDomain(IReadOnlyDictionary<Type, ISearchEngine> engines)
    {
        _engines = engines;
    }

    public ISearchEngine<TQueryData> Get<TQueryData>() where TQueryData : class
    {
        var targetType = typeof(TQueryData);
        if(_engines.TryGetValue(targetType, out var engine)) return (engine as ISearchEngine<TQueryData>)!;

        throw new SearchExpception($"No search engine registered for type: {targetType.FullName}");
    }

    public ISearchResult<TQueryData> Search<TQueryData>(string query) where TQueryData : class
    {
        var engine = Get<TQueryData>();
        
        var queryable = engine.Query(query, null);

        return new SearchResult<TQueryData>{
            Input = new SearchInput {
                Query = query,
                EvaluatedExpression = "",
                Commands = Array.Empty<string>()
            },

            Total = queryable.Count(),
            Content = queryable.ToArray()
        };
    }
    public Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(string query) where TQueryData : class
    {
        throw new NotImplementedException();
    }
}