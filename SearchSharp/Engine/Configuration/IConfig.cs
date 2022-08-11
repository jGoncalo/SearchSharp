using System.Linq.Expressions;
using SearchSharp.Engine.Rules;
using SearchSharp.Engine.Evaluation;
using Microsoft.Extensions.Logging;

namespace SearchSharp.Engine.Configuration;

public interface IConfig<TQueryData> where TQueryData : QueryData {
    IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
    Expression<Func<TQueryData, string, bool>> StringRule { get; }
    Expression<Func<TQueryData, bool>> DefaultHandler { get; }
    IEvaluator<TQueryData> Evaluator { get; }
    ILoggerFactory LoggerFactory { get; }
}