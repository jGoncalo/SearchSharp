using System.Linq.Expressions;

namespace SearchSharp.Engine.Data.Repository;

public interface IDataRepository<TQueryData> where TQueryData : QueryData {
    void Apply(Expression<Func<TQueryData, bool>> constraint);
    int Count();
    TQueryData[] Fetch();
}
