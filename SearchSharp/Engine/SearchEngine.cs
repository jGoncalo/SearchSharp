namespace SearchSharp.Engine;

using SearchSharp.Exceptions;
using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Evaluators;
using System.Collections.Generic;
using System.Linq.Expressions;
using SearchSharp.Items;
using SearchSharp.Items.Expressions;
using SearchSharp.Engine.Evaluators.Visitor;
using Sprache;
using Microsoft.Extensions.Logging;
using LinqExp = System.Linq.Expressions.Expression;
using SearchExp = SearchSharp.Items.Expressions.Expression;

public class SearchEngine<TQueryData> : ISearchEngine<TQueryData>
    where TQueryData : class {
    public class Builder {
        private readonly ISearchEngine<TQueryData>.IConfig _config;
        private Dictionary<string, ISearchEngine<TQueryData>.IDataProvider> _dataProviders = new();
        private string _defaultProvider = string.Empty;

        public Builder(ISearchEngine<TQueryData>.IConfig config){
            _config = config;
        }

        #region Providers
        public Builder RegisterProvider(ISearchEngine<TQueryData>.IDataProvider provider, bool isDefault = false){
            _dataProviders[provider.Name] = provider;

            if(string.IsNullOrWhiteSpace(_defaultProvider) || isDefault) _defaultProvider = provider.Name;
            return this;
        }
        public Builder RemoveProvider(string providerName) {
            var contains = _dataProviders.ContainsKey(providerName);
            if(contains) _dataProviders.Remove(providerName);
            return this;
        }
        public Builder ClearProviders(){
            _dataProviders.Clear();
            return this;
        }
        
        public Builder SetDefaultProvider(string providerName) {
            if(!_dataProviders.ContainsKey(providerName)) throw new ArgumentException($"Unknown provider - {providerName}", nameof(providerName));
            _defaultProvider = providerName;
            return this;
        }
        #endregion

        public SearchEngine<TQueryData> Build() {
            if(_dataProviders.Values.Count() == 0) throw new BuildException("At least one data provider must be registered");
            return new SearchEngine<TQueryData>(_config, _dataProviders, _defaultProvider);
        }
    }

    private readonly ISearchEngine<TQueryData>.IConfig _config;
    private readonly ISearchEngine<TQueryData>.IEvaluator _evaluator;
    private readonly IReadOnlyDictionary<string, ISearchEngine<TQueryData>.IDataProvider> _dataProviders;
    private readonly string _defaultProvider;
    private readonly ILogger<SearchEngine<TQueryData>> _logger;

    private SearchEngine(ISearchEngine<TQueryData>.IConfig config, 
        IReadOnlyDictionary<string, ISearchEngine<TQueryData>.IDataProvider> providers,
        string defaultProvider){
        _config = config;
        _evaluator = new Evaluator<TQueryData>(_config);
        _logger = _config.LoggerFactory.CreateLogger<SearchEngine<TQueryData>>();
        _dataProviders = providers;
        _defaultProvider = defaultProvider;
    }

    public IQueryable<TQueryData> Query(string query, string? dataProvider = null){
        if(string.IsNullOrWhiteSpace(query)) throw new ArgumentException("Null or empty argument", nameof(query));

        var foundProvider = _dataProviders.TryGetValue(dataProvider ?? _defaultProvider, out var provider);
        _logger.LogInformation("{Provider} [{Status}] -> {Query}",
            dataProvider,
            foundProvider ? "Found" : "Unknown",
            query);
        if(!foundProvider || provider == null) throw new Exception($"Data provider \"{dataProvider}\" not registred");
        try{
            var parseResult = QueryParser.Query.TryParse(query);
            if(!parseResult.WasSuccessful) throw new Exception(parseResult.Message);
            var parsedQuery = parseResult.Value;

            var queryLambda = FromQuery(parsedQuery);
            _logger.LogInformation("From query[{Query}] derived:\n{Expression}",
                query, queryLambda.ToString());
            var providerDataSource = ApplyRules(provider.DataSource(), EffectiveIn.Provider, parsedQuery.Commands);
            return ApplyRules(providerDataSource.Where(queryLambda), EffectiveIn.Query, parsedQuery.Commands);
        }
        catch(Exception exp){ 
            _logger.LogCritical(exp, "Unexpected error for query [{Query}]", query);
            throw new SearchExpception($"Unexpected error for query [{query}]", exp);
        }
    }

    private IQueryable<TQueryData> ApplyRules(IQueryable<TQueryData> query, EffectiveIn effectIn, IEnumerable<Command> commands){
        var afterQ = query;
        foreach(var command in commands) {
            if(_config.Commands.TryGetValue(command.Identifier, out var cmd) && cmd.EffectAt.HasFlag(effectIn)){
                var arguments = cmd.With(command.Arguments.Select(arg => arg.Literal).ToArray());
                try{
                    afterQ = cmd.Effect(query, arguments);
                }
                catch(Exception exp){
                    var argumentStr = arguments.Count() == 0 ? string.Empty : arguments.Values
                        .Select(arg => $"{arg.Identifier}[{arg.Literal.RawValue}]:{arg.Literal.Type}")
                        .Aggregate((left, right) => $"{left},{right}");
                    throw new CommandExecutionException($"Command execution failed: #{cmd.Identifier}({argumentStr})", exp);
                }
            }
        }
        return afterQ;
    }

    private Expression<Func<TQueryData, bool>> FromQuery(Query query) {
        var queryExpression = query.Root switch {
            LogicExpression compute => FromExpression(compute),
            StringExpression @string => FromExpression(@string),

            _ => throw new SearchExpception($"Unexpected query expression type: {query.GetType().Name}")
        };
        return new AssureQueryArgumentVisitor<TQueryData>().Assure(queryExpression);
    }

    private Expression<Func<TQueryData, bool>> FromExpression(StringExpression exp){
        var lambdaExpression = _evaluator.Evaluate(exp.Value);

        var argExpression = LinqExp.Parameter(typeof(TQueryData));
        return LinqExp.Lambda<Func<TQueryData, bool>>(lambdaExpression.Body,
            $"StringExpression", new[] {argExpression});
    }
    private Expression<Func<TQueryData, bool>> FromExpression(LogicExpression exp){
        return exp switch {
            Items.Expressions.BinaryExpression logic => FromExpression(logic),
            NegatedExpression neg => FromExpression(neg),
            DirectiveExpression dir => FromExpression(dir),
            SearchExp => FromExpression(exp),

            _ => throw new Exception($"Unexpected query expression type: {exp.GetType().Name}")
        };
    }
    private Expression<Func<TQueryData, bool>> FromExpression(Items.Expressions.BinaryExpression exp){
        var leftLambda = FromExpression(exp.Left);
        var rightLambda = FromExpression(exp.Right);

        var composedLambda = exp.Operator switch 
        {
            LogicOperator.Or => LinqExp.OrElse(leftLambda.Body, rightLambda.Body),
            LogicOperator.And => LinqExp.AndAlso(leftLambda.Body, rightLambda.Body),
            LogicOperator.Xor => LinqExp.ExclusiveOr(leftLambda.Body, rightLambda.Body),

            _ => throw new Exception($"unexpected OperationType value: {exp.Operator}")
        };

        return LinqExp.Lambda<Func<TQueryData, bool>>(composedLambda, 
            $"Logic[{leftLambda.Name}_{exp.Operator}_{rightLambda.Name}", leftLambda.Parameters);
    }
    private Expression<Func<TQueryData, bool>> FromExpression(NegatedExpression exp){
        if(exp.Negated == null) throw new Exception("Unexpected negative expression with null child");
        var lambdaExpression = FromExpression(exp.Negated);

        return LinqExp.Lambda<Func<TQueryData, bool>>(LinqExp.Not(lambdaExpression.Body),
            $"Not[{lambdaExpression.Name}]", lambdaExpression.Parameters);
    }
    private Expression<Func<TQueryData, bool>> FromExpression(DirectiveExpression exp){
        var lambdaExpression = exp.Directive switch {
            ComparisonDirective spec => _evaluator.Evaluate(spec),
            NumericDirective num => _evaluator.Evaluate(num),
            RangeDirective range => _evaluator.Evaluate(range),
            
            _ => throw new Exception($"Unexpected directive type: {exp.Directive.GetType().Name}")
        };

        return LinqExp.Lambda<Func<TQueryData, bool>>(lambdaExpression.Body, 
            $"Directive[{exp.Directive.Type}->{exp.Directive.Identifier}]", lambdaExpression.Parameters);
    }
}