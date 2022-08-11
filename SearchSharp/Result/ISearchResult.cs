using SearchSharp.Engine;

namespace SearchSharp.Result;

public interface ISearchResult<TQueryData> 
    where TQueryData : QueryData {
    public ISearchInput Input { get; }
    public int Total { get; }

    public TQueryData[] Content { get; }
}

public interface ISearchResult : ISearchResult<QueryData> {

}