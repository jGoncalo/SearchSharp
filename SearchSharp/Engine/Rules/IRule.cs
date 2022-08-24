using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Rules;

/// <summary>
/// Specification for a given query rule
/// </summary>
/// <typeparam name="TQueryData">Query data type</typeparam>
public interface IRule<TQueryData> where TQueryData : QueryData {
    /// <summary>
    /// Unique identifier (case sensitive)
    /// </summary>
    string Identifier { get; } 
    /// <summary>
    /// Description
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Comparison expressions for String literals
    /// </summary>
    IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> ComparisonStrRules { get; }
    /// <summary>
    /// Comparison expressions for Numeric literals
    /// </summary>
    IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> ComparisonNumRules { get; }
    /// <summary>
    /// Comparison expressions for Boolean literals
    /// </summary>
    IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, BooleanLiteral, bool>>> ComparisonBoolRules { get; }
    /// <summary>
    /// Numeric expressions
    /// </summary>
    IReadOnlyDictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> NumericRules { get; }
    /// <summary>
    /// Range expression
    /// </summary>
    Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? RangeRule { get; }
    /// <summary>
    /// List expression for String lists
    /// </summary>
    Expression<Func<TQueryData, StringLiteral[], bool>>? StringListRule { get; }
    /// <summary>
    /// List expression for Numeric lists
    /// </summary>
    Expression<Func<TQueryData, NumericLiteral[], bool>>? NumericListRule { get; }
    /// <summary>
    /// List expression for Boolean lists
    /// </summary>
    Expression<Func<TQueryData, BooleanLiteral[], bool>>? BooleanListRule { get; }
}