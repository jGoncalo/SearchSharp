using SearchSharp.Engine;
using Microsoft.EntityFrameworkCore;

namespace SearchSharp.EntityFramework;

public static class Extensions {
    public static SearchEngine<TQueryData> AddEntityFrameworkProvider<TContext, TQueryData>(this SearchEngine<TQueryData> se,
        Func<TContext> contextFactory,
        Func<TContext, IQueryable<TQueryData>> selector,
        string? providerName = null) 
        where TQueryData : class 
        where TContext : DbContext {
            se.RegisterProvider(new ContextProvider<TContext, TQueryData>(contextFactory, selector, providerName));
        return se;
    }
    public static SearchEngine<TQueryData> AddEntityFrameworkProvider<TContext, TQueryData>(this SearchEngine<TQueryData> se,
        Func<TContext> contextFactory,
        string? providerName = null) 
        where TQueryData : class 
        where TContext : DbContext {
            se.RegisterProvider(new ContextProvider<TContext, TQueryData>(contextFactory, 
                context => context.Set<TQueryData>().AsNoTracking(), providerName));
        return se;
    }
}