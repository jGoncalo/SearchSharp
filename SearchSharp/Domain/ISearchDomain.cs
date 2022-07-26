using SearchSharp.Engine;

namespace SearchSharp.Domain;

public interface ISearchDomain {
    ISearchEngine<TQueryData> Get<TQueryData>() where TQueryData : class;

    ISearchResult<TQueryData> Search<TQueryData>(string query) where TQueryData : class;
    Task<ISearchResult<TQueryData>> SearchAsync<TQueryData>(string query) where TQueryData : class;
}