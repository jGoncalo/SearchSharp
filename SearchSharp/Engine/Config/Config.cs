using System.Linq.Expressions;
using SearchSharp.Engine.Rules;
using SearchSharp.Engine.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SearchSharp.Engine.Config;

public class Config<TQueryData> : ISearchEngine<TQueryData>.IConfig 
    where TQueryData : QueryData {
    public class Builder {
        private readonly Dictionary<string, ICommand<TQueryData>> _commands = new();
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

        #region Commands
        public Builder AddCommand(ICommand<TQueryData> command) {
            _commands[command.Identifier] = command;
            return this;
        }
        public Builder WithCommand(string identifier, Action<Command<TQueryData>.Builder> config){
            var builder = Command<TQueryData>.Builder.For(identifier);
            config(builder);
            _commands[identifier] = builder.Build();
            return this;
        }
        public Builder WithCommand<TCommandSpec>() where TCommandSpec : CommandTemplate<TQueryData>, new() {
            var templatedCommand = new Command<TQueryData, TCommandSpec>();
            _commands[templatedCommand.Identifier] = templatedCommand;
            return this;
        }
        public Builder RemoveCommand(string commandIdentifier) {
            if(_commands.ContainsKey(commandIdentifier)) _commands.Remove(commandIdentifier);
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
            return new Config<TQueryData>(_commands, _rules, _stringRule, _defaultHandler, _loggerFactory);
        }
    }

    public IReadOnlyDictionary<string, ICommand<TQueryData>> Commands { get; }
    public IReadOnlyDictionary<string, IRule<TQueryData>> Rules { get; }
    public Expression<Func<TQueryData, string, bool>> StringRule { get; }
    public Expression<Func<TQueryData, bool>> DefaultHandler { get; }

    public ILoggerFactory LoggerFactory { get; }

    public Config(IReadOnlyDictionary<string, ICommand<TQueryData>> commands,
        IReadOnlyDictionary<string, IRule<TQueryData>> rules,
        Expression<Func<TQueryData, string, bool>> stringRule,
        Expression<Func<TQueryData, bool>> defaultHandler,
        ILoggerFactory loggerFactory) {
        Commands = commands;
        Rules = rules;
        StringRule = stringRule;
        DefaultHandler = defaultHandler;
        LoggerFactory = loggerFactory;
    }
}