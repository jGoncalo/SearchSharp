using System.Linq.Expressions;
using SearchSharp.Items;
using SearchSharp.Engine.Rules;

namespace SearchSharp.Engine;

public interface ISearchEngine<TQueryData> where TQueryData : class {
    public interface IEvaluator {
        Expression<Func<TQueryData, bool>> Evaluate(ComparisonDirective directive);
        Expression<Func<TQueryData, bool>> Evaluate(NumericDirective directive);
        Expression<Func<TQueryData, bool>> Evaluate(RangeDirective directive);
        Expression<Func<TQueryData, bool>> Evaluate(string textQuery);
    }
    public interface IDataProvider {
        string Name { get; }

        Task<IQueryable<TQueryData>> DataSourceAsync(CancellationToken ct = default);
        IQueryable<TQueryData> DataSource();
    }
    public interface IConfig {
        IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
        Expression<Func<TQueryData, string, bool>> StringRule { get; }
        Expression<Func<TQueryData, bool>> DefaultHandler { get; }
    }

    SearchEngine<TQueryData> RegisterProvider(IDataProvider provider);
    SearchEngine<TQueryData> RemoveProvider(IDataProvider provider);
    SearchEngine<TQueryData> RemoveProvider(string providerName);

    IQueryable<TQueryData> Query(string dataProvider, string query);
}