using SearchSharp.Engine.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SearchSharp.EntityFramework;

public class ContextRepository<TContext, TQueryData> : IRepository<TQueryData, IQueryable<TQueryData>>
    where TQueryData : QueryData
    where TContext : DbContext
{
    public delegate IQueryable<TQueryData> Selector(TContext context);

    private readonly TContext _context;
    public IQueryable<TQueryData> DataSet { get; private set; }

    public ContextRepository(TContext context, Selector setSelector){
        _context = context;
        DataSet = setSelector(context);
    }

    public void Modify(Func<IQueryable<TQueryData>, IQueryable<TQueryData>> modifer) {
        DataSet = modifer(DataSet);
    }
    public Task ModifyAsync(Func<IQueryable<TQueryData>, IQueryable<TQueryData>> modifer, CancellationToken ct = default){
        Modify(modifer);
        return Task.CompletedTask;
    }

    public void Apply(Expression<Func<TQueryData, bool>> condition)
    {
        DataSet = DataSet.Where(condition);
    }
    public Task ApplyAsync(Expression<Func<TQueryData, bool>> condition, CancellationToken ct = default) {
        Apply(condition);
        return Task.CompletedTask;
    }

    public int Count() => DataSet.Count();
    public Task<int> CountAsync(CancellationToken ct = default) => DataSet.CountAsync(ct);

    public TQueryData[] Fetch() => DataSet.ToArray();
    public Task<TQueryData[]> FetchAsync(CancellationToken ct = default) => Task.FromResult(Fetch());
}