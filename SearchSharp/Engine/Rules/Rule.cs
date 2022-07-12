using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Rules;

public class Rule<TQueryData> : IRule<TQueryData> where TQueryData : class {
    public class Builder {
        public readonly string Identifier;
        private string _description = string.Empty;

        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> _comparisonStrRules = new();
        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> _comparisonNumRules = new();
        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, BooleanLiteral, bool>>> _comparisonBoolRules = new();
        private readonly Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> _numericRules = new();
        private Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? _rangeRule;

        public Builder(string identifier) {
            Identifier = identifier;
        }

        public Builder AddDescription(string description) {
            _description = description;
            return this;
        }

        public Builder AddOperator<TLiteral>(DirectiveComparisonOperator @operator, Expression<Func<TQueryData, TLiteral, bool>> rule) 
            where TLiteral : Literal {
            if (typeof(TLiteral) == typeof(BooleanLiteral)) _comparisonBoolRules[@operator] = (rule as Expression<Func<TQueryData, BooleanLiteral, bool>>)!;
            else if (typeof(TLiteral) == typeof(StringLiteral)) _comparisonStrRules[@operator] = (rule as Expression<Func<TQueryData, StringLiteral, bool>>)!;
            else if (typeof(TLiteral) == typeof(NumericLiteral)) _comparisonNumRules[@operator] = (rule as Expression<Func<TQueryData, NumericLiteral, bool>>)!;

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
            return new Rule<TQueryData>(Identifier, _description, _comparisonStrRules, _comparisonNumRules, _comparisonBoolRules, _numericRules, _rangeRule);
        }
    }

    public string Identifier { get; }
    public string Description { get; }
    public IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> ComparisonStrRules { get; }
    public IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> ComparisonNumRules { get; }
    public IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, BooleanLiteral, bool>>> ComparisonBoolRules { get; }
    public IReadOnlyDictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> NumericRules { get; }
    public Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? RangeRule { get; }

    private Rule(string identifier, string description,
        IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> comparisonStrRules,
        IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> comparisonNumRules,
        IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, BooleanLiteral, bool>>> comparisonBoolRules,
        IReadOnlyDictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> numericRules,
        Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? rangeRule) {
        Identifier = identifier;
        Description = description;
        ComparisonStrRules = comparisonStrRules;
        ComparisonNumRules = comparisonNumRules;
        ComparisonBoolRules = comparisonBoolRules;
        NumericRules = numericRules;
        RangeRule = rangeRule;
    }
}