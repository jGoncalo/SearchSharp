using System.Linq.Expressions;
using SearchSharp.Engine.Rules;
using SearchSharp.Engine.Evaluation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SearchSharp.Engine.Configuration;

public class Config<TQueryData> : IConfig<TQueryData>
    where TQueryData : QueryData {
    public class Builder {
        private readonly Dictionary<string, IRule<TQueryData>> _rules = new();

        private Expression<Func<TQueryData, string, bool>> _stringRule = (data, query) => data.ToString()!.Contains(query);
        private IEvaluator<TQueryData>? _evaluator = null;
        private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

        public Builder() {
        }

        #region Rules
        public Builder AddRule(IRule<TQueryData> rule) {
            _rules[rule.Identifier] = rule;
            return this;
        }
        public Builder WithRule(string identifier, Action<Rule<TQueryData>.Builder> config) {
            var builder = Rule<TQueryData>.Builder.For(identifier);
            config(builder);
            _rules[identifier] = builder.Build();
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
        #endregion

        public Builder SetEvaluator(IEvaluator<TQueryData> evaluator) {
            _evaluator = evaluator;
            return this;
        }

        public Builder AddLogger(ILoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
            return this;
        }

        public Config<TQueryData> Build() {
            var evaluator = _evaluator ?? new Evaluator<TQueryData>(
                _rules,
                _stringRule
            );
            return new Config<TQueryData>(_rules, _stringRule, evaluator, _loggerFactory);
        }
    }

    public IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
    public Expression<Func<TQueryData, string, bool>> StringRule { get; }

    public IEvaluator<TQueryData> Evaluator { get; }
    public ILoggerFactory LoggerFactory { get; }

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