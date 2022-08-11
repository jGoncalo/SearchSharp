using SearchSharp.Result;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine;

public interface ISearchEngine {
    public Type DataType { get; }
    public string Alias { get; }

    ISearchResult<QueryData> Query(string query, string? dataProvider = null);
    ISearchResult<QueryData> Query(Query query, string? dataProvider = null);
}

public interface ISearchEngine<TQueryData> : ISearchEngine where TQueryData : QueryData {
    new ISearchResult<TQueryData> Query(string query, string? dataProvider = null);
    new ISearchResult<TQueryData> Query(Query query, string? dataProvider = null);
}
