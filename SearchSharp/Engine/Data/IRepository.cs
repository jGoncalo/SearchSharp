using System.Linq.Expressions;

namespace SearchSharp.Engine.Data;

public interface IRepository<TQueryData, TDataStructure> 
    where TQueryData : QueryData 
    where TDataStructure : class {

    public TDataStructure DataSet { get; }

    void Modify(Func<TDataStructure, TDataStructure> modifer);
    void Apply(Expression<Func<TQueryData, bool>> condition);
    int Count();
    TQueryData[] Fetch();
}