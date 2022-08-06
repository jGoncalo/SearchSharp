using SearchSharp.Engine;
using SearchSharp.Engine.Data;
using SearchSharp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SearchSharp.EntityFramework;

public class ContextRepositoryFactory<TContext, TQueryData> : IDataProviderFactory<TQueryData, ContextRepository<TContext, TQueryData>>
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

public class ContextRepository<TContext, TQueryData> : IDataRepository<TQueryData>
    where TQueryData : QueryData
    where TContext : DbContext
{
    public delegate IQueryable<TQueryData> Selector(TContext context);
    public delegate IQueryable<TQueryData> Modify(IQueryable<TQueryData> query);

    private readonly TContext _context;
    private IQueryable<TQueryData> _query;

    public ContextRepository(TContext context, Selector setSelector){
        _context = context;
        _query = setSelector(context);
    }

    public void Apply(Modify modifer){
        _query = modifer(_query);
    }
    public void Apply(Expression<Func<TQueryData, bool>> condition)
    {
        _query = _query.Where(condition);
    }

    public int Count() => _query.Count();

    public TQueryData[] Fetch() => _query.ToArray();
}