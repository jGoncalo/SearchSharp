using System.Linq.Expressions;
using SearchSharp.Engine.Repositories;

namespace SearchSharp.Memory;

public class MemoryRepository<TQueryData> : IRepository<TQueryData, IQueryable<TQueryData>>
    where TQueryData : QueryData {

    public IQueryable<TQueryData> DataSet { get; private set;}

    internal MemoryRepository(IQueryable<TQueryData> data) {
        DataSet = data;
    }

    public void Modify(Func<IQueryable<TQueryData>, IQueryable<TQueryData>> modifer) {
        DataSet = modifer(DataSet);
    }
    public Task ModifyAsync(Func<IQueryable<TQueryData>, IQueryable<TQueryData>> modifer, CancellationToken ct = default) {
        Modify(modifer);
        return Task.CompletedTask;
    }

    public void Apply(Expression<Func<TQueryData, bool>> condition)
    {
        DataSet = DataSet.Where(condition);
    }
    public Task ApplyAsync(Expression<Func<TQueryData, bool>> condition, CancellationToken ct = default)
    {
        Apply(condition);
        return Task.CompletedTask;
    }

    public int Count() => DataSet.Count();
    public Task<int> CountAsync(CancellationToken ct = default) => Task.FromResult(Count());

    public TQueryData[] Fetch() => DataSet.ToArray();
    public Task<TQueryData[]> FetchAsync(CancellationToken ct = default) => Task.FromResult(Fetch());
}
