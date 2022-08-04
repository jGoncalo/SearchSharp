using System.Linq.Expressions;

namespace SearchSharp.Engine.Data.Repository;

public class QueryRepository<TQueryData> : IDataRepository<TQueryData> where TQueryData : QueryData {
    private IQueryable<TQueryData> _query;

    public QueryRepository(IQueryable<TQueryData> query) {
        _query = query;
    }

    public void Apply(Expression<Func<TQueryData, bool>> constraint) {
        _query = _query.Where(constraint);
    }
    public int Count() => _query.Count();
    public TQueryData[] Fetch() => _query.ToArray();
}