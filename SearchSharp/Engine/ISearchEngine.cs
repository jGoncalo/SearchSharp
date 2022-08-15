using SearchSharp.Result;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine;

public interface ISearchEngine {
    public Type DataType { get; }
    public string Alias { get; }

    ISearchResult Query(string query, string? dataProvider = null);
    ISearchResult Query(Query query, string? dataProvider = null);

    Task<ISearchResult> QueryAsync(string query, string? dataProvider = null, CancellationToken ct = default);
    Task<ISearchResult> QueryAsync(Query query, string? dataProvider = null, CancellationToken ct = default);
}

public interface ISearchEngine<TQueryData> : ISearchEngine where TQueryData : QueryData {
    new ISearchResult<TQueryData> Query(string query, string? dataProvider = null);
    new Task<ISearchResult<TQueryData>> QueryAsync(string query, string? dataProvider = null, CancellationToken ct = default);
    
    new ISearchResult<TQueryData> Query(Query query, string? dataProvider = null);
    new Task<ISearchResult<TQueryData>> QueryAsync(Query query, string? dataProvider = null, CancellationToken ct = default);
}
