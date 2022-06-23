using System.Linq.Expressions;
using SearchSharp.Engine.Rules;

namespace SearchSharp.Engine.Config;

public class Config<TQueryData> : ISearchEngine<TQueryData>.IConfig 
    where TQueryData : class {
    public class Builder {
        private readonly Dictionary<string, IRule<TQueryData>> _rules = new();

        private Expression<Func<TQueryData, string, bool>> _stringRule = (data, query) => data.ToString()!.Contains(query);
        private Expression<Func<TQueryData, bool>> _defaultHandler = _ => false;

        public Builder() {
        }

        public Builder AddRule(IRule<TQueryData> rule) {
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
    
        public Config<TQueryData> Build() {
            return new Config<TQueryData>(_rules, _stringRule, _defaultHandler);
        }
    }

    public IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
    public Expression<Func<TQueryData, string, bool>> StringRule { get; }
    public Expression<Func<TQueryData, bool>> DefaultHandler { get; }

    public Config(IReadOnlyDictionary<string, IRule<TQueryData>> rules,
        Expression<Func<TQueryData, string, bool>> stringRule,
        Expression<Func<TQueryData, bool>> defaultHandler) {
        Rules = rules;
        StringRule = stringRule;
        DefaultHandler = defaultHandler;
    }
}