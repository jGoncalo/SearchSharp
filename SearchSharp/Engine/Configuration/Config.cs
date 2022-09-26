using System.Linq.Expressions;
using SearchSharp.Engine.Rules;
using SearchSharp.Engine.Evaluation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SearchSharp.Engine.Configuration;

/// <summary>
/// Specification for a given Search configuration
/// </summary>
/// <typeparam name="TQueryData">Data type associated with configuration</typeparam>
public class Config<TQueryData> : IConfig<TQueryData>
    where TQueryData : QueryData {
    /// <summary>
    /// Configuration builder class (fluet pattern)
    /// </summary>
    public class Builder {
        private readonly Dictionary<string, IRule<TQueryData>> _rules = new();

        private Expression<Func<TQueryData, string, bool>> _stringRule = (data, query) => data.ToString()!.Contains(query);
        private IEvaluator<TQueryData>? _evaluator = null;
        private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

        /// <summary>
        /// Create a new Configuration builder
        /// </summary>
        public Builder() {
        }

        #region Rules
        /// <summary>
        /// Register a new rule (upsert on rule.Identifier)
        /// </summary>
        /// <param name="rule">Rule specification</param>
        /// <returns>This builder</returns>
        public Builder AddRule(IRule<TQueryData> rule) {
            _rules[rule.Identifier] = rule;
            return this;
        }
        /// <summary>
        /// Register a new rule (upsert on identifier)
        /// </summary>
        /// <param name="identifier">Unique identifier of the rule</param>
        /// <param name="config">Action for a rule builder configuration</param>
        /// <returns>This builder</returns>
        public Builder WithRule(string identifier, Action<Rule<TQueryData>.Builder> config) {
            var builder = Rule<TQueryData>.Builder.For(identifier);
            config(builder);
            _rules[identifier] = builder.Build();
            return this;
        }
        /// <summary>
        /// Remove rule (if any existis with identifier)
        /// </summary>
        /// <param name="ruleIdentifier">Unique rule identifier</param>
        /// <returns>This builder</returns>
        public Builder RemoveRule(string ruleIdentifier) {
            if(_rules.ContainsKey(ruleIdentifier)) _rules.Remove(ruleIdentifier);
            return this;
        }

        /// <summary>
        /// Sets a string rule
        /// </summary>
        /// <param name="rule">C# expression that data must obey</param>
        /// <returns>This builder</returns>
        public Builder SetStringRule(Expression<Func<TQueryData, string, bool>> rule){
            _stringRule = rule;
            return this;
        }
        /// <summary>
        /// Restore default string rule
        /// (ex: data.ToString() must contain string)
        /// </summary>
        /// <returns>This builder</returns>
        public Builder ResetStringRule() {
    	    _stringRule = (data, query) => data.ToString()!.Contains(query);
            return this;
        }
        #endregion

        /// <summary>
        /// Set Evaluator used to transform DQL into C# expression
        /// </summary>
        /// <param name="evaluator">Evaluator</param>
        /// <returns>This builder</returns>
        public Builder SetEvaluator(IEvaluator<TQueryData> evaluator) {
            _evaluator = evaluator;
            return this;
        }

        /// <summary>
        /// Add a logger factory 
        /// </summary>
        /// <param name="loggerFactory">Logger factory to be used</param>
        /// <returns>This builder</returns>
        public Builder AddLogger(ILoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Craete a Configuration based on specified rules
        /// (if no evaluator was specified, default SearchSharp is used)
        /// </summary>
        /// <returns>Search Sharp Configuration</returns>
        public Config<TQueryData> Build() {
            var evaluator = _evaluator ?? new Evaluator<TQueryData>(
                _rules,
                _stringRule
            );
            return new Config<TQueryData>(_rules, _stringRule, evaluator, _loggerFactory);
        }
    }

    /// <summary>
    /// Rules associated with this configuration
    /// </summary>
    public IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
    
    /// <summary>
    /// String rule associated with this configuration
    /// </summary>
    public Expression<Func<TQueryData, string, bool>> StringRule { get; }

    
    /// <summary>
    /// Evaluator used to transform a DQL query into C# expression
    /// </summary>
    public IEvaluator<TQueryData> Evaluator { get; }
    
    /// <summary>
    /// Logger factory associated with this configuration
    /// </summary>
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Create a new configuration
    /// </summary>
    /// <param name="rules">rules for this configuration</param>
    /// <param name="stringRule">string rule for this configuration</param>
    /// <param name="evaluator">evaluator to use in this configuration</param>
    /// <param name="loggerFactory">logger factory for this configuration</param>
    public Config(IReadOnlyDictionary<string, IRule<TQueryData>> rules,
        Expression<Func<TQueryData, string, bool>> stringRule,
        IEvaluator<TQueryData> evaluator,
        ILoggerFactory loggerFactory) {
        Rules = rules;
        StringRule = stringRule;
        Evaluator = evaluator;
        LoggerFactory = loggerFactory;
    }
}