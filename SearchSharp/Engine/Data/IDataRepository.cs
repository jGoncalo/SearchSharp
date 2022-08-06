using System.Linq.Expressions;

namespace SearchSharp.Engine.Data;

public interface IDataProviderFactory<TQueryData, TDataRepo> 
    where TQueryData : QueryData
    where TDataRepo : IDataRepository<TQueryData> {
        TDataRepo Instance();
    }

public interface IDataRepository<TQueryData> where TQueryData : QueryData {
    void Apply(Expression<Func<TQueryData, bool>> condition);
    int Count();
    TQueryData[] Fetch();
}