using System.Linq.Expressions;
using SearchSharp.Engine.Rules;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SearchSharp.Engine.Configuration;

public class Config<TQueryData> : IConfig<TQueryData>
    where TQueryData : QueryData {
    public class Builder {
        private readonly Dictionary<string, IRule<TQueryData>> _rules = new();

        private Expression<Func<TQueryData, string, bool>> _stringRule = (data, query) => data.ToString()!.Contains(query);
        private Expression<Func<TQueryData, bool>> _defaultHandler = _ => false;
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

        public Builder AddLogger(ILoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
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
    
        public Config<TQueryData> Build() {
            return new Config<TQueryData>(_rules, _stringRule, _defaultHandler, _loggerFactory);
        }
    }

    public IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
    public Expression<Func<TQueryData, string, bool>> StringRule { get; }
    public Expression<Func<TQueryData, bool>> DefaultHandler { get; }

    public ILoggerFactory LoggerFactory { get; }

    public Config(IReadOnlyDictionary<string, IRule<TQueryData>> rules,
        Expression<Func<TQueryData, string, bool>> stringRule,
        Expression<Func<TQueryData, bool>> defaultHandler,
        ILoggerFactory loggerFactory) {
        Rules = rules;
        StringRule = stringRule;
        DefaultHandler = defaultHandler;
        LoggerFactory = loggerFactory;
    }
}