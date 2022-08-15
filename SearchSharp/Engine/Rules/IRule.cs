using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Rules;

public interface IRule<TQueryData> where TQueryData : QueryData {
    string Identifier { get; } 
    string Description { get; }

    IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> ComparisonStrRules { get; }
    IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> ComparisonNumRules { get; }
    IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, BooleanLiteral, bool>>> ComparisonBoolRules { get; }
    IReadOnlyDictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> NumericRules { get; }
    Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? RangeRule { get; }
    Expression<Func<TQueryData, StringLiteral[], bool>>? StringListRule { get; }
    Expression<Func<TQueryData, NumericLiteral[], bool>>? NumericListRule { get; }
    Expression<Func<TQueryData, BooleanLiteral[], bool>>? BooleanListRule { get; }
}