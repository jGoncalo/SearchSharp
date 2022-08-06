using SearchSharp.Engine;
using SearchSharp.Engine.Data;
using Microsoft.EntityFrameworkCore;

namespace SearchSharp.EntityFramework;

public static class Extensions {

    public static SearchEngine<TQueryData>.Builder AddEntityFrameworkProvider<TContext, TQueryData>(this SearchEngine<TQueryData>.Builder builder,
        IDbContextFactory<TContext> contextFactory,
        string name = "entityFramework",
        bool isDefault = false,
        Action<DataProvider<TQueryData, ContextRepository<TContext, TQueryData>>.Builder>? config = null) 
        where TQueryData : QueryData
        where TContext : DbContext {

        var repoFactory = new ContextRepositoryFactory<TContext, TQueryData>(contextFactory, (context) => context.Set<TQueryData>());

        var providerBuilder = new DataProvider<TQueryData, ContextRepository<TContext, TQueryData>>.Builder(name, repoFactory);
        if(config != null) config(providerBuilder);
        builder.RegisterProvider(providerBuilder.Build());

        return builder;
    }

    public static SearchEngine<TQueryData>.Builder AddEntityFrameworkProvider<TContext, TQueryData>(this SearchEngine<TQueryData>.Builder builder,
        IDbContextFactory<TContext> contextFactory,
        ContextRepository<TContext, TQueryData>.Selector selector,
        string name = "entityFramework",
        bool isDefault = false,
        Action<DataProvider<TQueryData, ContextRepository<TContext, TQueryData>>.Builder>? config = null) 
        where TQueryData : QueryData
        where TContext : DbContext {
            
        var repoFactory = new ContextRepositoryFactory<TContext, TQueryData>(contextFactory, selector);

        var providerBuilder = new DataProvider<TQueryData, ContextRepository<TContext, TQueryData>>.Builder(name, repoFactory);
        if(config != null) config(providerBuilder);
        builder.RegisterProvider(providerBuilder.Build());

        return builder;
    }
}