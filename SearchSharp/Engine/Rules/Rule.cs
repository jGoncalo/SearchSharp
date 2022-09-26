using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Rules;

/// <summary>
/// Specification for a given query rule
/// </summary>
/// <typeparam name="TQueryData">Query data type</typeparam>
public class Rule<TQueryData> : IRule<TQueryData> where TQueryData : QueryData {
    /// <summary>
    /// Rule builder
    /// </summary>
    public class Builder {
        /// <summary>
        /// Unique rule identifier
        /// </summary>
        public readonly string Identifier;
        private string _description = string.Empty;

        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> _comparisonStrRules = new();
        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> _comparisonNumRules = new();
        private readonly Dictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, BooleanLiteral, bool>>> _comparisonBoolRules = new();
        private readonly Dictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> _numericRules = new();
        private Expression<Func<TQueryData, StringLiteral[], bool>>? _strListRule;
        private Expression<Func<TQueryData, NumericLiteral[], bool>>? _numListRule;
        private Expression<Func<TQueryData, BooleanLiteral[], bool>>? _boolListRule;
        private Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? _rangeRule;

        private Builder(string identifier) {
            Identifier = identifier;
        }

        /// <summary>
        /// Builder for a unique rule identiifer
        /// </summary>
        /// <param name="identifier">Unique rule identifier</param>
        /// <returns>Rule builder</returns>
        public static Builder For(string identifier) => new Builder(identifier);

        /// <summary>
        /// Add description to rule
        /// </summary>
        /// <param name="description">Rule description</param>
        /// <returns>This builder</returns>
        public Builder AddDescription(string description) {
            _description = description;
            return this;
        }

        /// <summary>
        /// Register Comparison Directive
        /// </summary>
        /// <typeparam name="TLiteral">Literal type for rule</typeparam>
        /// <param name="operator">Rule operator</param>
        /// <param name="rule">Rule C# expression (aka constraint)</param>
        /// <returns>This builder</returns>
        public Builder AddOperator<TLiteral>(DirectiveComparisonOperator @operator, Expression<Func<TQueryData, TLiteral, bool>> rule) 
            where TLiteral : Literal {
            if (typeof(TLiteral) == typeof(BooleanLiteral)) _comparisonBoolRules[@operator] = (rule as Expression<Func<TQueryData, BooleanLiteral, bool>>)!;
            else if (typeof(TLiteral) == typeof(StringLiteral)) _comparisonStrRules[@operator] = (rule as Expression<Func<TQueryData, StringLiteral, bool>>)!;
            else if (typeof(TLiteral) == typeof(NumericLiteral)) _comparisonNumRules[@operator] = (rule as Expression<Func<TQueryData, NumericLiteral, bool>>)!;

            return this;
        }

        /// <summary>
        /// Register Numeric Directive
        /// </summary>
        /// <param name="operator">Numeric operator</param>
        /// <param name="rule">Rule C# expression (aka constraint)</param>
        /// <returns>This builder</returns>
        public Builder AddOperator(DirectiveNumericOperator @operator, Expression<Func<TQueryData, NumericLiteral, bool>> rule) {
            _numericRules[@operator] = rule;
            return this;
        }
        
        /// <summary>
        /// Register range directive
        /// </summary>
        /// <param name="rule">Rule C# expression (aka constraint)</param>
        /// <returns>This builder</returns>
        public Builder AddOperator(Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>> rule) {
            _rangeRule = rule;
            return this;
        }

        /// <summary>
        /// Register List Directive
        /// </summary>
        /// <typeparam name="TLiteral">Type of list literal</typeparam>
        /// <param name="rule">Rule C# expression (aka constraint)</param>
        /// <returns>This builder</returns>
        public Builder AddOperator<TLiteral>(Expression<Func<TQueryData, TLiteral[], bool>> rule)
            where TLiteral : Literal {
            if(typeof(TLiteral) == typeof(StringLiteral))
                _strListRule = rule as Expression<Func<TQueryData, StringLiteral[], bool>>;
            if(typeof(TLiteral) == typeof(NumericLiteral))
                _numListRule = rule as Expression<Func<TQueryData, NumericLiteral[], bool>>;
            if(typeof(TLiteral) == typeof(BooleanLiteral))
                _boolListRule = rule as Expression<Func<TQueryData, BooleanLiteral[], bool>>;

            return this;
        }

        /// <summary>
        /// Build Rule
        /// </summary>
        /// <returns>Rule</returns>
        public Rule<TQueryData> Build() {
            return new Rule<TQueryData>(Identifier, _description, _comparisonStrRules, _comparisonNumRules, _comparisonBoolRules, _numericRules,  _strListRule, _numListRule, _boolListRule, _rangeRule);
        }
    }

    /// <summary>
    /// Unique identifier (case sensitive)
    /// </summary>
    public string Identifier { get; }
    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; }
    /// <summary>
    /// Comparison expressions for String literals
    /// </summary>
    public IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> ComparisonStrRules { get; }
    /// <summary>
    /// Comparison expressions for Numeric literals
    /// </summary>
    public IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> ComparisonNumRules { get; }
    /// <summary>
    /// Comparison expressions for Boolean literals
    /// </summary>
    public IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, BooleanLiteral, bool>>> ComparisonBoolRules { get; }
    /// <summary>
    /// Numeric expressions
    /// </summary>
    public IReadOnlyDictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> NumericRules { get; }
    /// <summary>
    /// List expression for String lists
    /// </summary>
    public Expression<Func<TQueryData, StringLiteral[], bool>>? StringListRule { get; }
    /// <summary>
    /// List expression for Numeric lists
    /// </summary>
    public Expression<Func<TQueryData, NumericLiteral[], bool>>? NumericListRule { get; }
    /// <summary>
    /// List expression for Boolean lists
    /// </summary>
    public Expression<Func<TQueryData, BooleanLiteral[], bool>>? BooleanListRule { get; }
    /// <summary>
    /// Range expression
    /// </summary>
    public Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? RangeRule { get; }

    private Rule(string identifier, string description,
        IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, StringLiteral, bool>>> comparisonStrRules,
        IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> comparisonNumRules,
        IReadOnlyDictionary<DirectiveComparisonOperator, Expression<Func<TQueryData, BooleanLiteral, bool>>> comparisonBoolRules,
        IReadOnlyDictionary<DirectiveNumericOperator, Expression<Func<TQueryData, NumericLiteral, bool>>> numericRules,
        Expression<Func<TQueryData, StringLiteral[], bool>>? stringListRule,
        Expression<Func<TQueryData, NumericLiteral[], bool>>? numericListRule,
        Expression<Func<TQueryData, BooleanLiteral[], bool>>? boolListRule,
        Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>? rangeRule) {
        Identifier = identifier;
        Description = description;
        ComparisonStrRules = comparisonStrRules;
        ComparisonNumRules = comparisonNumRules;
        ComparisonBoolRules = comparisonBoolRules;
        NumericRules = numericRules;
        StringListRule = stringListRule;
        NumericListRule = numericListRule;
        BooleanListRule = boolListRule;
        RangeRule = rangeRule;
    }
}