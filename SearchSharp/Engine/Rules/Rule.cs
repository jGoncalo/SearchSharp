using SearchSharp.Items;
using System.Linq.Expressions;

namespace SearchSharp.Engine.Rules;

public interface IRule<TQueryData> where TQueryData : class {
    string Identifier { get; } 
    string Description { get; }

    Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> ComparisonStrRules { get; }
    Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> ComparisonNumRules { get; }
    Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> NumericRules { get; }
    Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? RangeRule { get; }
}

public class Rule<TQueryData> : IRule<TQueryData> where TQueryData : class {
    public class Builder {
        public readonly string Identifier;
        private string _description = string.Empty;

        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> _comparisonStrRules = new();
        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> _comparisonNumRules = new();
        private readonly Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> _numericRules = new();
        private Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? _rangeRule;

        public Builder(string identifier) {
            Identifier = identifier;
        }

        public Builder AddDescription(string description) {
            _description = description;
            return this;
        }

        public Builder AddOperator(DirectiveComparisonOperator @operator, Expression<Func<TQueryData, StringLiteral, bool>> rule){
            _comparisonStrRules[@operator] = rule;
            return this;
        }
        public Builder AddOperator(DirectiveComparisonOperator @operator, Expression<Func<TQueryData, NumericLiteral, bool>> rule){
            _comparisonNumRules[@operator] = rule;
            return this;
        }

        public Builder AddOperator(DirectiveNumericOperator @operator, Expression<Func<TQueryData, NumericLiteral, bool>> rule) {
            _numericRules[@operator] = rule;
            return this;
        }
    
        public Builder AddOperator(Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>> rule) {
            _rangeRule = rule;
            return this;
        }

        public Rule<TQueryData> Build() {
            return new Rule<TQueryData>(Identifier, _description, _comparisonStrRules, _comparisonNumRules, _numericRules, _rangeRule);
        }
    }

    public string Identifier { get; }
    public string Description { get; }
    public Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> ComparisonStrRules { get; } = new();
    public Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> ComparisonNumRules { get; } = new();
    public Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> NumericRules { get; } = new();
    public Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? RangeRule { get; }

    private Rule(string identifier, string description,
        Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> comparisonStrRules,
        Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> comparisonNumRules,
        Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> numericRules,
        Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? rangeRule) {
        Identifier = identifier;
        Description = description;
        ComparisonStrRules = comparisonStrRules;
        ComparisonNumRules = comparisonNumRules;
        NumericRules = numericRules;
        RangeRule = rangeRule;
    }
}