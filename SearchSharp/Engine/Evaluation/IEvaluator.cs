using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components.Directives;

namespace SearchSharp.Engine.Evaluation;

public interface IEvaluator<TQueryData> where TQueryData : QueryData {
    Expression<Func<TQueryData, bool>> Evaluate(ComparisonDirective directive);
    Expression<Func<TQueryData, bool>> Evaluate(NumericDirective directive);
    Expression<Func<TQueryData, bool>> Evaluate(RangeDirective directive);
    Expression<Func<TQueryData, bool>> Evaluate(ListDirective directive);
    Expression<Func<TQueryData, bool>> Evaluate(string textQuery);
}
