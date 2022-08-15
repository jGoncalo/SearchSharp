using SearchSharp.Result;
using SearchSharp.Engine;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Domain;

public interface ISearchDomain {
    ISearchEngine this[string alias] { get; }

    bool TryGet(string alias, out ISearchEngine? engine);
    bool TryGet<TQueryData>(string alias, out ISearchEngine<TQueryData>? engine) where TQueryData : QueryData;

    ISearchResult Search(string query, string? engineAlias = null, string? dataProvider = null);
    ISearchResult Search(Query query, string? engineAlias = null, string? dataProvider = null);
    Task<ISearchResult> SearchAsync(string query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default);
    Task<ISearchResult> SearchAsync(Query query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default);

    ISearchResult<TQueryData> Search<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData;
    ISearchResult<TQueryData> Search<TQueryData>(Query query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData;
    Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default) where TQueryData : QueryData;
    Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(Query query, string? engineAlias = null, string? dataProvider = null, CancellationToken ct = default) where TQueryData : QueryData;
}