using SearchSharp.Engine;
using SearchSharp.Engine.Data;
using SearchSharp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SearchSharp.EntityFramework;

public class ContextRepositoryFactory<TContext, TQueryData> : IRepositoryFactory<TQueryData, ContextRepository<TContext, TQueryData>, IQueryable<TQueryData>>
     where TQueryData : QueryData
     where TContext : DbContext
{
    private readonly IDbContextFactory<TContext> _contextFactory;
    private readonly ContextRepository<TContext, TQueryData>.Selector _selector;

    public ContextRepositoryFactory(IDbContextFactory<TContext> contextFactory, 
        ContextRepository<TContext, TQueryData>.Selector selector) {
        _contextFactory = contextFactory;
        _selector = selector;
    }

    public ContextRepository<TContext, TQueryData> Instance()
    {
        return new ContextRepository<TContext, TQueryData>(_contextFactory.CreateDbContext(), _selector);
    }
}

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

    public void Apply(Expression<Func<TQueryData, bool>> condition)
    {
        DataSet = DataSet.Where(condition);
    }

    public int Count() => DataSet.Count();

    public TQueryData[] Fetch() => DataSet.ToArray();
}