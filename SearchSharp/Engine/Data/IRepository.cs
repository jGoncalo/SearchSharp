using System.Linq.Expressions;

namespace SearchSharp.Engine.Data;

public interface IRepository<TQueryData, TDataStructure> 
    where TQueryData : QueryData 
    where TDataStructure : class {

    public TDataStructure DataSet { get; }

    Task ModifyAsync(Func<TDataStructure, TDataStructure> modifer, CancellationToken ct = default);
    void Modify(Func<TDataStructure, TDataStructure> modifer);

    Task ApplyAsync(Expression<Func<TQueryData, bool>> condition, CancellationToken ct = default);
    void Apply(Expression<Func<TQueryData, bool>> condition);

    Task<int> CountAsync(CancellationToken ct = default);
    int Count();

    Task<TQueryData[]> FetchAsync(CancellationToken ct = default);
    TQueryData[] Fetch();

}