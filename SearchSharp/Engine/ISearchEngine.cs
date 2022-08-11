using System.Linq.Expressions;
using SearchSharp.Engine.Rules;
using SearchSharp.Engine.Commands;
using Microsoft.Extensions.Logging;
using SearchSharp.Domain;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine;

public interface ISearchEngine {
    public Type DataType { get; }
    public string Alias { get; }

    ISearchResult<QueryData> Query(string query, string? dataProvider = null);
    ISearchResult<QueryData> Query(Query query, string? dataProvider = null);
}

public interface ISearchEngine<TQueryData> : ISearchEngine where TQueryData : QueryData {
    public interface IEvaluator {
        Expression<Func<TQueryData, bool>> Evaluate(ComparisonDirective directive);
        Expression<Func<TQueryData, bool>> Evaluate(NumericDirective directive);
        Expression<Func<TQueryData, bool>> Evaluate(RangeDirective directive);
        Expression<Func<TQueryData, bool>> Evaluate(ListDirective directive);
        Expression<Func<TQueryData, bool>> Evaluate(string textQuery);
    }
    public interface IConfig {
        IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
        Expression<Func<TQueryData, string, bool>> StringRule { get; }
        Expression<Func<TQueryData, bool>> DefaultHandler { get; }
        ILoggerFactory LoggerFactory { get; }
    }

    new ISearchResult<TQueryData> Query(string query, string? dataProvider = null);
    new ISearchResult<TQueryData> Query(Query query, string? dataProvider = null);
}