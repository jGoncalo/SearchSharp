using SearchSharp.Engine.Rules;
using SearchSharp.Engine.Parser.Components.Literals;
using SearchSharp.Engine.Parser.Components.Directives;
using SearchSharp.Exceptions;
using System.Linq.Expressions;
using SearchSharp.Engine.Evaluation.Visitor;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Evaluation;

/// <summary>
/// Evaluate a given directive into a simple expression
/// </summary>
/// <typeparam name="TQueryData">Data type</typeparam>
public class Evaluator<TQueryData> : IEvaluator<TQueryData> where TQueryData : QueryData {
    private IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
    private Expression<Func<TQueryData, string, bool>> StringRule { get; }

    /// <summary>
    /// Create an evaluator
    /// </summary>
    /// <param name="rules">rule definitions to be replaced on evaluation</param>
    /// <param name="stringRule">string rule definition to be replaced on evaluation</param>
    public Evaluator(IReadOnlyDictionary<string, IRule<TQueryData>> rules, 
        Expression<Func<TQueryData, string, bool>> stringRule) {
        Rules = rules;
        StringRule = stringRule;
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
        if(arguments.IsStringList is false) throw new ArgumentResolutionException("String list composition requires all literals to be StringLiteral");
        var stringArguments = arguments.Literals.Cast<StringLiteral>().ToArray();

        var visited = new ReplaceListVisitor<TQueryData, StringLiteral>(stringArguments).Replace(listRule);
        return visited;
    }
    private static Expression<Func<TQueryData, bool>> ComposeList(Expression<Func<TQueryData, NumericLiteral[], bool>> listRule,
        Arguments arguments){
        if(arguments.IsNumericList is false) throw new ArgumentResolutionException("String list composition requires all literals to be NumericLiteral");
        var stringArguments = arguments.Literals.Cast<NumericLiteral>().ToArray();

        var visited = new ReplaceListVisitor<TQueryData, NumericLiteral>(stringArguments).Replace(listRule);
        return visited;
    }
    private static Expression<Func<TQueryData, bool>> ComposeList(Expression<Func<TQueryData, BooleanLiteral[], bool>> listRule,
        Arguments arguments){
        if(arguments.IsBooleanList is false) throw new ArgumentResolutionException("String list composition requires all literals to be BooleanLiteral");
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

    /// <summary>
    /// Evaluate directive into common expression
    /// </summary>
    /// <param name="directive">Directive specification</param>
    /// <returns>Common expression</returns>
    public Expression<Func<TQueryData, bool>> Evaluate(ComparisonDirective directive) {
        var hasRule = Rules.TryGetValue(directive.Identifier, out var rule);
        
        if(!hasRule) throw new UnknownRuleException(directive.Identifier);

        Expression<Func<TQueryData, bool>> lambda;

        switch(directive.Value) {
            case StringLiteral strLit:
                lambda = rule!.ComparisonStrRules.TryGetValue(directive.Operator, out var exactStrRule) ?
                    ComposeComparison(exactStrRule, strLit) : throw new UnknownRuleDirectiveException(directive);
                break;
            case NumericLiteral numLit:
                lambda = rule!.ComparisonNumRules.TryGetValue(directive.Operator, out var exactNumRule) ?
                    ComposeComparison(exactNumRule, numLit) : throw new UnknownRuleDirectiveException(directive);
                break;
            case BooleanLiteral boolLit:
                lambda = rule!.ComparisonBoolRules.TryGetValue(directive.Operator, out var exactBoolRule) ?
                    ComposeComparison(exactBoolRule, boolLit) : throw new UnknownRuleDirectiveException(directive);
                break;
            default:
                throw new UnknownRuleDirectiveException(directive);
        }

        return lambda;
    }
    /// <summary>
    /// Evaluate directive into common expression
    /// </summary>
    /// <param name="directive">Directive specification</param>
    /// <returns>Common expression</returns>
    public Expression<Func<TQueryData, bool>> Evaluate(NumericDirective directive) {

        var hasRule = Rules.TryGetValue(directive.Identifier, out var rule);
        if(!hasRule) throw new UnknownRuleException(directive.Identifier);

        var hasOpRule = rule!.NumericRules.TryGetValue(directive.OperatorSpec.Type, out var opRule);
        if(!hasOpRule) throw new UnknownRuleDirectiveException(directive);

        return ComposeNumeric(opRule!, directive.OperatorSpec.Value);
    }
    /// <summary>
    /// Evaluate directive into common expression
    /// </summary>
    /// <param name="directive">Directive specification</param>
    /// <returns>Common expression</returns>
    public Expression<Func<TQueryData, bool>> Evaluate(RangeDirective directive) {
        var hasRule = Rules.TryGetValue(directive.Identifier, out var rule);
        if(!hasRule) throw new UnknownRuleException(directive.Identifier);

        var rangeRule = rule!.RangeRule;
        if(rangeRule == null) throw new UnknownRuleDirectiveException(directive);

        return ComposeRange(rangeRule, directive.OperatorSpec.LowerBound, directive.OperatorSpec.UpperBound);
    }
    /// <summary>
    /// Evaluate directive into common expression
    /// </summary>
    /// <param name="directive">Directive specification</param>
    /// <returns>Common expression</returns>
    public Expression<Func<TQueryData, bool>> Evaluate(ListDirective directive) {
        var hasRule = Rules.TryGetValue(directive.Identifier, out var rule);
        if(!hasRule) throw new UnknownRuleException(directive.Identifier);

        var strListRule = rule!.StringListRule;
        if(strListRule is not null) 
            return ComposeList(strListRule, directive.Arguments);
        
        var numListRule = rule!.NumericListRule;
        if(numListRule is not null)
            return ComposeList(numListRule, directive.Arguments);

        var boolListRule = rule!.BooleanListRule;
        if(boolListRule is not null)
            return ComposeList(boolListRule, directive.Arguments);

        throw new UnknownRuleDirectiveException(directive);
    }
    /// <summary>
    /// Evaluate text into common expression
    /// </summary>
    /// <param name="textQuery">generic text</param>
    /// <returns>Common expression</returns>
    public Expression<Func<TQueryData, bool>> Evaluate(string textQuery) {
        return ComposeText(StringRule, textQuery);
    }
    #endregion

}