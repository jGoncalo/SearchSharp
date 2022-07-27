using SearchSharp.Engine;

namespace SearchSharp.Domain;

public interface ISearchDomain {
    ISearchEngine this[string alias] { get; }

    bool TryGet(string alias, out ISearchEngine? engine);
    bool TryGet<TQueryData>(string alias, out ISearchEngine<TQueryData>? engine) where TQueryData : QueryData;

    ISearchResult Search(string query, string? engineAlias = null, string? dataProvider = null);
    Task<ISearchResult> SearchAsync(string query, string? engineAlias = null, string? dataProvider = null);

    ISearchResult<TQueryData> Search<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData;
    Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(string query, string? engineAlias = null, string? dataProvider = null) where TQueryData : QueryData;
}