using System.Linq.Expressions;
using SearchSharp.Engine.Rules;
using SearchSharp.Engine.Evaluation;
using Microsoft.Extensions.Logging;

namespace SearchSharp.Engine.Configuration;

/// <summary>
/// Specification for a given Search configuration
/// </summary>
/// <typeparam name="TQueryData">Data type associated with configuration</typeparam>
public interface IConfig<TQueryData> where TQueryData : QueryData {
    /// <summary>
    /// Rules associated with this configuration
    /// </summary>
    IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
    /// <summary>
    /// String rule associated with this configuration
    /// </summary>
    Expression<Func<TQueryData, string, bool>> StringRule { get; }
    /// <summary>
    /// Evaluator used to transform a DQL query into C# expression
    /// </summary>
    IEvaluator<TQueryData> Evaluator { get; }
    /// <summary>
    /// Logger factory associated with this configuration
    /// </summary>
    ILoggerFactory LoggerFactory { get; }
}