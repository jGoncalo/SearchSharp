using SearchSharp.Engine.Repositories;
using Microsoft.EntityFrameworkCore;

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
