using SearchSharp.Engine;
using SearchSharp.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SearchSharp.EntityFramework;

public class ContextProvider<TContext, TQueryData> : ISearchEngine<TQueryData>.IDataProvider
    where TQueryData : QueryData
    where TContext : DbContext
{
        public string Name { get; }

        private readonly Func<TContext> _contextFactory;
        private readonly Func<TContext, IQueryable<TQueryData>> _selector;

        public Task<IQueryable<TQueryData>> DataSourceAsync(CancellationToken ct = default){
            return Task.FromResult(DataSource());
        }
        public IQueryable<TQueryData> DataSource(){
            try{
                var context = _contextFactory() ?? throw new SearchExpception("Failed to acquire context instance from factory");
                return _selector(context) ?? throw new SearchExpception("Failed to acquire IQueryable from context");
            }
            catch(SearchExpception){
                throw;
            }
            catch(Exception exp){
                throw new SearchExpception("Failed to acquire data source", exp);
            }
        }

        public ContextProvider(Func<TContext> contextFactory,
            Func<TContext, IQueryable<TQueryData>> selector,
            string? name = null) {
            Name = name ?? "EntityFrameworkProvider";
            _contextFactory = contextFactory;
            _selector = selector;
        }
}
