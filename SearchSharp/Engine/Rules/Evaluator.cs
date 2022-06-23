using SearchSharp.Items;
using SearchSharp.Engine.Rules.Visitor;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SearchSharp.Engine.Rules;

public class Evaluator<TQueryData> : SearchEngine<TQueryData>.IEvaluator where TQueryData : class {
    public class Builder {
        private readonly Dictionary<string, Rule<TQueryData>> _rules = new();

        private Expression<Func<TQueryData, string, bool>> _stringRule = (data, query) => data.ToString()!.Contains(query);
        private Expression<Func<TQueryData, bool>> _defaultHandler = _ => false;

        public Builder() {
        }

        public Builder AddRule(Rule<TQueryData> rule) {
            _rules[rule.Identifier] = rule;
            return this;
        }
        public Builder RemoveRule(string ruleIdentifier) {
            if(_rules.ContainsKey(ruleIdentifier)) _rules.Remove(ruleIdentifier);
            return this;
        }

        public Builder SetStringRule(Expression<Func<TQueryData, string, bool>> rule){
            _stringRule = rule;
            return this;
        }
        public Builder ResetStringRule() {
    	    _stringRule = (data, query) => data.ToString()!.Contains(query);
            return this;
        }

        public Builder SetDefaultHandler(Expression<Func<TQueryData, bool>> rule){
            _defaultHandler = rule;
            return this;
        }
        public Builder ResetDefaultHanlder() {
            _defaultHandler = _ => false;
            return this;
        }
    
        public Evaluator<TQueryData> Build() {
            return new Evaluator<TQueryData>(_rules, _stringRule, _defaultHandler);
        }
    }

    private readonly IReadOnlyDictionary<string, Rule<TQueryData>> _rules;
    private readonly Expression<Func<TQueryData, string, bool>> _stringRule;
    private readonly Expression<Func<TQueryData, bool>> _defaultHandler;

    private Evaluator(IReadOnlyDictionary<string, Rule<TQueryData>> rules,
        Expression<Func<TQueryData, string, bool>> stringRule,
        Expression<Func<TQueryData, bool>> defaultHandler) {
        _rules = rules;
        _stringRule = stringRule;
        _defaultHandler = defaultHandler;
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
        var hasRule = _rules.TryGetValue(directive.Identifier, out var rule);
        
        if(!hasRule) return _defaultHandler;

        Expression<Func<TQueryData, bool>> lambda;

        switch(directive.Value) {
            case StringLiteral strLit:
                lambda = rule!.ComparisonStrRules.TryGetValue(directive.OperatorType, out var exactStrRule) ?
                    ComposeComparison(exactStrRule, strLit) : _defaultHandler;
                break;
            case NumericLiteral numLit:
                lambda = rule!.ComparisonNumRules.TryGetValue(directive.OperatorType, out var exactNumRule) ?
                    ComposeComparison(exactNumRule, numLit) : _defaultHandler;
                break;
            default:
                lambda = _defaultHandler;
                break;
        }

        return lambda;
    }
    public Expression<Func<TQueryData, bool>> Evaluate(NumericDirective directive) {

        var hasRule = _rules.TryGetValue(directive.Identifier, out var rule);
        if(!hasRule) return _defaultHandler;

        var hasOpRule = rule!.NumericRules.TryGetValue(directive.OperatorSpec.OperatorType, out var opRule);
        if(!hasOpRule) return _defaultHandler;

        return ComposeNumeric(opRule!, directive.OperatorSpec.Value);
    }
    public Expression<Func<TQueryData, bool>> Evaluate(RangeDirective directive) {
        var hasRule = _rules.TryGetValue(directive.Identifier, out var rule);
        if(!hasRule) return _defaultHandler;

        var rangeRule = rule!.RangeRule;
        if(rangeRule == null) return _defaultHandler;

        return ComposeRange(rangeRule, directive.OperatorSpec.LowerBound, directive.OperatorSpec.UpperBound);
    }
    public Expression<Func<TQueryData, bool>> Evaluate(string textQuery) {
        return ComposeText(_stringRule, textQuery);
    }
    #endregion

}