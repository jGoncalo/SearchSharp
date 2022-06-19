using SearchSharp.Items;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SearchSharp.Engine.Rules;

public class Rule<TQueryData> where TQueryData : class {
    public class Builder {
        public readonly string Identifier;

        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> _comparisonStrRules = new();
        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> _comparisonNumRules = new();
        private readonly Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> _numericRules = new();
        private Expression<Func<TQueryData, NumericLiteral?, NumericLiteral?, bool>>? _rangeRule;

        public Builder(string identifier) {
            Identifier = identifier;
        }

        public Builder AddStringOperator(DirectiveComparisonOperator @operator, Expression<Func<TQueryData, StringLiteral, bool>> rule){
            _comparisonStrRules[@operator] = rule;
            return this;
        }
        public Builder AddNumericOperator(DirectiveComparisonOperator @operator, Expression<Func<TQueryData, NumericLiteral, bool>> rule){
            _comparisonNumRules[@operator] = rule;
            return this;
        }

        public Builder AddOperator(DirectiveNumericOperator @operator, Expression<Func<TQueryData, NumericLiteral, bool>> rule) {
            _numericRules[@operator] = rule;
            return this;
        }
    
        public Builder AddRange(Expression<Func<TQueryData, NumericLiteral?, NumericLiteral?, bool>> rule) {
            _rangeRule = rule;
            return this;
        }

        public Rule<TQueryData> Build() {
            return new Rule<TQueryData>(Identifier, _comparisonStrRules, _comparisonNumRules, _numericRules, _rangeRule);
        }
    }

    public readonly string Identifier;
    public readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> ComparisonStrRules = new();
    public readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> ComparisonNumRules = new();
    public readonly Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> NumericRules = new();
    public readonly Expression<Func<TQueryData, NumericLiteral?, NumericLiteral?, bool>>? RangeRule;

    private Rule(string identifier, 
        Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> comparisonStrRules,
        Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> comparisonNumRules,
        Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> numericRules,
        Expression<Func<TQueryData, NumericLiteral?, NumericLiteral?, bool>>? rangeRule) {
        Identifier = identifier;
        ComparisonStrRules = comparisonStrRules;
        ComparisonNumRules = comparisonNumRules;
        NumericRules = numericRules;
        RangeRule = rangeRule;
    }
}