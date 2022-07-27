using SearchSharp.Engine;
using Microsoft.EntityFrameworkCore;

namespace SearchSharp.EntityFramework;

public static class Extensions {
    public static SearchEngine<TQueryData>.Builder AddEntityFrameworkProvider<TContext, TQueryData>(this SearchEngine<TQueryData>.Builder builder,
        Func<TContext> contextFactory,
        Func<TContext, IQueryable<TQueryData>> selector,
        string? providerName = null) 
        where TQueryData : QueryData 
        where TContext : DbContext {
            builder.RegisterProvider(new ContextProvider<TContext, TQueryData>(contextFactory, selector, providerName));
        return builder;
    }
    public static SearchEngine<TQueryData>.Builder AddEntityFrameworkProvider<TContext, TQueryData>(this SearchEngine<TQueryData>.Builder builder,
        Func<TContext> contextFactory,
        string? providerName = null) 
        where TQueryData : QueryData 
        where TContext : DbContext {
            builder.RegisterProvider(new ContextProvider<TContext, TQueryData>(contextFactory, 
                context => context.Set<TQueryData>().AsNoTracking(), providerName));
        return builder;
    }
}