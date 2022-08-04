using SearchSharp.Engine.Parser.Components;
using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine;

using SearchSharp.Exceptions;
using SearchSharp.Engine.Config;
using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Evaluators;
using System.Collections.Generic;
using System.Linq.Expressions;
using SearchSharp.Engine.Evaluators.Visitor;
using SearchSharp.Domain;
using SearchSharp.Engine.Data;
using Sprache;
using Microsoft.Extensions.Logging;
using LinqExp = System.Linq.Expressions.Expression;
using SearchExp = Expression;

public class SearchEngine<TQueryData> : ISearchEngine<TQueryData>
    where TQueryData : QueryData {
    public class Builder {
        private readonly string _alias;
        private ISearchEngine<TQueryData>.IConfig _config;
        private Dictionary<string, IDataProvider<TQueryData>> _dataProviders = new Dictionary<string, IDataProvider<TQueryData>>();
        private string _defaultProvider = string.Empty;

        public Builder(string alias){
            _alias = alias;
            _config = new Config<TQueryData>.Builder().Build();
        }

        #region Configuration
        public Builder With(ISearchEngine<TQueryData>.IConfig config) {
            _config = config;
            return this;
        }
        public Builder With(Action<Config<TQueryData>.Builder> configuration){
            var builder = new Config<TQueryData>.Builder();
            configuration(builder);
            _config = builder.Build();
            return this;
        }
        #endregion

        #region Providers
        public Builder RegisterProvider(IDataProvider<TQueryData> provider, bool isDefault = false){
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
            return new SearchEngine<TQueryData>(_alias, _config, _dataProviders, _defaultProvider);
        }
    }

    public Type DataType { get; }
    public String Alias { get; }

    private readonly ISearchEngine<TQueryData>.IConfig _config;
    private readonly ISearchEngine<TQueryData>.IEvaluator _evaluator;
    private readonly IReadOnlyDictionary<string, IDataProvider<TQueryData>> _dataProviders;
    private readonly string _defaultProvider;
    private readonly ILogger<SearchEngine<TQueryData>> _logger;

    private SearchEngine(string alias, ISearchEngine<TQueryData>.IConfig config, 
        IReadOnlyDictionary<string, IDataProvider<TQueryData>> providers,
        string defaultProvider){
        Alias = alias;
        DataType = typeof(TQueryData);
        
        _config = config;
        _evaluator = new Evaluator<TQueryData>(_config);
        _logger = _config.LoggerFactory.CreateLogger<SearchEngine<TQueryData>>();
        _dataProviders = providers;
        _defaultProvider = defaultProvider;
    }

    ISearchResult<QueryData> ISearchEngine.Query(string query, string? dataProvider)
    {
        var result = Query(query, dataProvider);
        return new SearchResult<QueryData>{
            Input = result.Input,
            Total = result.Total,

            Content = result.Content.Cast<QueryData>().ToArray()
        };
    }
    ISearchResult<QueryData> ISearchEngine.Query(Query query, string? dataProvider)
    {
        var result = Query(query, dataProvider);
        return new SearchResult<QueryData>{
            Input = result.Input,
            Total = result.Total,

            Content = result.Content.Cast<QueryData>().ToArray()
        };
    }

    public ISearchResult<TQueryData> Query(string query, string? dataProvider = null){
        var parseResult = QueryParser.Query.TryParse(query);
        if(!parseResult.WasSuccessful) throw new SearchExpception(parseResult.Message);

        return Query(parseResult.Value, dataProvider);
    }
    public ISearchResult<TQueryData> Query(Query query, string? dataProvider = null){
        try{
            var targetProvider = query.Provider?.ProviderId ?? dataProvider ?? _defaultProvider;
            
            var foundProvider = _dataProviders.TryGetValue(targetProvider, out var provider);
            _logger.LogInformation("{Provider} [{Status}] -> {Query}",
                targetProvider,
                foundProvider ? "Found" : "Unknown",
                query);
            if(!foundProvider || provider == null) throw new SearchExpception($"Data provider \"{targetProvider}\" not registred");

            var queryLambda = FromQuery(query);
            _logger.LogInformation("From query[{Query}] derived:\n{Expression}",
                query, queryLambda.ToString());

            return provider.ExecuteQuery(query, queryLambda);
        }
        catch(Exception exp){ 
            _logger.LogCritical(exp, "Unexpected error for query [{Query}]", query);
            throw new SearchExpception($"Unexpected error for query [{query}]", exp);
        }
    }

    private Expression<Func<TQueryData, bool>> FromQuery(Query query) {
        var queryExpression = query.Root switch {
            StringExpression @string => FromExpression(@string),
            LogicExpression compute => FromExpression(compute),

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
            Parser.Components.Expressions.BinaryExpression logic => FromExpression(logic),
            NegatedExpression neg => FromExpression(neg),
            DirectiveExpression dir => FromExpression(dir),
            StringExpression str => FromExpression(str),
            SearchExp => FromExpression(exp),

            _ => throw new Exception($"Unexpected query expression type: {exp.GetType().Name}")
        };
    }
    private Expression<Func<TQueryData, bool>> FromExpression(Parser.Components.Expressions.BinaryExpression exp){
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