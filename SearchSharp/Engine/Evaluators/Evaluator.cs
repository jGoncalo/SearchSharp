using SearchSharp.Engine.Rules;
using SearchSharp.Exceptions;
using System.Linq.Expressions;
using SearchSharp.Engine.Evaluators.Visitor;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Evaluators;

public class Evaluator<TQueryData> : ISearchEngine<TQueryData>.IEvaluator where TQueryData : QueryData {
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
    private static Expression<Func<TQueryData, bool>> ComposeComparison(Expression<Func<TQueryData, BooleanLiteral, bool>> booleanRule,
        BooleanLiteral literal) {
        var visited = new ReplaceLiteralVisitor<TQueryData, BooleanLiteral>(literal).Replace(booleanRule);
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
    private static Expression<Func<TQueryData, bool>> ComposeList(Expression<Func<TQueryData, StringLiteral[], bool>> listRule,
        Arguments arguments){
        if(arguments.IsStringList is false) throw new ArgumentResolutionException("TODO");
        var stringArguments = arguments.Literals.Cast<StringLiteral>().ToArray();

        var visited = new ReplaceListVisitor<TQueryData, StringLiteral>(stringArguments).Replace(listRule);
        return visited;
    }
    private static Expression<Func<TQueryData, bool>> ComposeList(Expression<Func<TQueryData, NumericLiteral[], bool>> listRule,
        Arguments arguments){
        if(arguments.IsNumericList is false) throw new ArgumentResolutionException("TODO");
        var stringArguments = arguments.Literals.Cast<NumericLiteral>().ToArray();

        var visited = new ReplaceListVisitor<TQueryData, NumericLiteral>(stringArguments).Replace(listRule);
        return visited;
    }
    private static Expression<Func<TQueryData, bool>> ComposeList(Expression<Func<TQueryData, BooleanLiteral[], bool>> listRule,
        Arguments arguments){
        if(arguments.IsBooleanList is false) throw new ArgumentResolutionException("TODO");
        var stringArguments = arguments.Literals.Cast<BooleanLiteral>().ToArray();

        var visited = new ReplaceListVisitor<TQueryData, BooleanLiteral>(stringArguments).Replace(listRule);
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
            case BooleanLiteral boolLit:
                lambda = rule!.ComparisonBoolRules.TryGetValue(directive.OperatorType, out var exactBoolRule) ?
                    ComposeComparison(exactBoolRule, boolLit) : DefaultHandler;
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
    public Expression<Func<TQueryData, bool>> Evaluate(ListDirective directive) {
        var hasRule = Rules.TryGetValue(directive.Identifier, out var rule);
        if(!hasRule) return DefaultHandler;

        var strListRule = rule!.StringListRule;
        if(strListRule is not null) 
            return ComposeList(strListRule, directive.Arguments);
        
        var numListRule = rule!.NumericListRule;
        if(numListRule is not null)
            return ComposeList(numListRule, directive.Arguments);

        var boolListRule = rule!.BooleanListRule;
        if(boolListRule is not null)
            return ComposeList(boolListRule, directive.Arguments);

        return DefaultHandler;
    }
    public Expression<Func<TQueryData, bool>> Evaluate(string textQuery) {
        return ComposeText(StringRule, textQuery);
    }
    #endregion

}