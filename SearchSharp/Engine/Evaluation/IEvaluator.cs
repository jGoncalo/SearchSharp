using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components.Directives;

namespace SearchSharp.Engine.Evaluation;

/// <summary>
/// Evaluate a given directive into a simple expression
/// </summary>
/// <typeparam name="TQueryData">Data type</typeparam>
public interface IEvaluator<TQueryData> where TQueryData : QueryData {
    /// <summary>
    /// Evaluate directive into common expression
    /// </summary>
    /// <param name="directive">Directive specification</param>
    /// <returns>Common expression</returns>
    Expression<Func<TQueryData, bool>> Evaluate(ComparisonDirective directive);
    /// <summary>
    /// Evaluate directive into common expression
    /// </summary>
    /// <param name="directive">Directive specification</param>
    /// <returns>Common expression</returns>
    Expression<Func<TQueryData, bool>> Evaluate(NumericDirective directive);
    /// <summary>
    /// Evaluate directive into common expression
    /// </summary>
    /// <param name="directive">Directive specification</param>
    /// <returns>Common expression</returns>
    Expression<Func<TQueryData, bool>> Evaluate(RangeDirective directive);
    /// <summary>
    /// Evaluate directive into common expression
    /// </summary>
    /// <param name="directive">Directive specification</param>
    /// <returns>Common expression</returns>
    Expression<Func<TQueryData, bool>> Evaluate(ListDirective directive);
    /// <summary>
    /// Evaluate text into common expression
    /// </summary>
    /// <param name="textQuery">generic text</param>
    /// <returns>Common expression</returns>
    Expression<Func<TQueryData, bool>> Evaluate(string textQuery);
}
