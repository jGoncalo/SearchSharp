using System.Linq.Expressions;
using SearchSharp.Items;
using SearchSharp.Engine.Rules;
using Microsoft.Extensions.Logging;

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
        ILoggerFactory LoggerFactory { get; }
    }

    IQueryable<TQueryData> Query(string query, string? dataProvider = null);
}