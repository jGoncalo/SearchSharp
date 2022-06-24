using SearchSharp.Items;
using SearchSharp.Engine.Rules;
using System.Linq.Expressions;
using SearchSharp.Engine.Evaluators.Visitor;

namespace SearchSharp.Engine.Evaluators;

public class Evaluator<TQueryData> : ISearchEngine<TQueryData>.IEvaluator where TQueryData : class {
    private readonly ISearchEngine<TQueryData>.IConfig _config;

    private IReadOnlyDictionary<string, IRule<TQueryData>> Rules => _config.Rules;
    private Expression<Func<TQueryData, string, bool>> StringRule => _config.StringRule;
    private Expression<Func<TQueryData, bool>> DefaultHandler => _config.DefaultHandler;

    public Evaluator(ISearchEngine<TQueryData>.IConfig config) {
        _config = config;
    }

    #region Evaluation
    #region Compose Lambda
    private static Expression<Func<TQueryData, bool>> ComposeComparison(Expression<Func<TQueryData, StringLiteral, bool>> stringRule,
        StringLiteral literal) {
        var visited = new ReplaceLiteralVisitor<TQueryData, StringLiteral>(literal).Replace(stringRule);
        return visited;
    }
    private static Expression<Func<TQueryData, bool>> ComposeComparison(Expression<Func<TQueryData, NumericLiteral, bool>> numericRule,
        NumericLiteral literal) {
        var visited = new ReplaceLiteralVisitor<TQueryData, NumericLiteral>(literal).Replace(numericRule);
        return visited;
    }
    private static Expression<Func<TQueryData, bool>> ComposeNumeric(Expression<Func<TQueryData, NumericLiteral, bool>> numericRule,
        NumericLiteral literal){
        var visited = new ReplaceLiteralVisitor<TQueryData, NumericLiteral>(literal).Replace(numericRule);
        return visited;
    }
    private static Expression<Func<TQueryData, bool>> ComposeRange(Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>> rangeRule,
        NumericLiteral lower, NumericLiteral upper){
        var visited = new ReplaceRangeVisitor<TQueryData>(lower, upper).Replace(rangeRule);
        return visited;
    }
    private static Expression<Func<TQueryData, bool>> ComposeText(Expression<Func<TQueryData, string, bool>> rule,
        string text){ 
        var visited = new ReplaceStringVisitor<TQueryData>(text).Replace(rule);
        return visited;
    }
    #endregion 

    public Expression<Func<TQueryData, bool>> Evaluate(ComparisonDirective directive) {
        var hasRule = Rules.TryGetValue(directive.Identifier, out var rule);
        
        if(!hasRule) return DefaultHandler;

        Expression<Func<TQueryData, bool>> lambda;

        switch(directive.Value) {
            case StringLiteral strLit:
                lambda = rule!.ComparisonStrRules.TryGetValue(directive.OperatorType, out var exactStrRule) ?
                    ComposeComparison(exactStrRule, strLit) : DefaultHandler;
                break;
            case NumericLiteral numLit:
                lambda = rule!.ComparisonNumRules.TryGetValue(directive.OperatorType, out var exactNumRule) ?
                    ComposeComparison(exactNumRule, numLit) : DefaultHandler;
                break;
            default:
                lambda = DefaultHandler;
                break;
        }

        return lambda;
    }
    public Expression<Func<TQueryData, bool>> Evaluate(NumericDirective directive) {

        var hasRule = Rules.TryGetValue(directive.Identifier, out var rule);
        if(!hasRule) return DefaultHandler;

        var hasOpRule = rule!.NumericRules.TryGetValue(directive.OperatorSpec.OperatorType, out var opRule);
        if(!hasOpRule) return DefaultHandler;

        return ComposeNumeric(opRule!, directive.OperatorSpec.Value);
    }
    public Expression<Func<TQueryData, bool>> Evaluate(RangeDirective directive) {
        var hasRule = Rules.TryGetValue(directive.Identifier, out var rule);
        if(!hasRule) return DefaultHandler;

        var rangeRule = rule!.RangeRule;
        if(rangeRule == null) return DefaultHandler;

        return ComposeRange(rangeRule, directive.OperatorSpec.LowerBound, directive.OperatorSpec.UpperBound);
    }
    public Expression<Func<TQueryData, bool>> Evaluate(string textQuery) {
        return ComposeText(StringRule, textQuery);
    }
    #endregion

}