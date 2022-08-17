using SearchSharp.Engine.Parser.Components;
using SearchSharp.Engine.Parser.Components.Expressions;
using SearchSharp.Engine.Parser.Components.Directives;
using SearchSharp.Result;

namespace SearchSharp.Engine;

using SearchSharp.Exceptions;
using SearchSharp.Engine.Configuration;
using SearchSharp.Engine.Providers;
using SearchSharp.Engine.Parser;
using SearchSharp.Engine.Evaluation;
using SearchSharp.Engine.Evaluation.Visitor;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sprache;
using Microsoft.Extensions.Logging;
using LinqExp = System.Linq.Expressions.Expression;
using SearchExp = Expression;

public class SearchEngine<TQueryData> : ISearchEngine<TQueryData>
    where TQueryData : QueryData {
    public class Builder {
        private readonly string _alias;
        private IConfig<TQueryData> _config;
        private Dictionary<string, IProvider<TQueryData>> _dataProviders = new Dictionary<string, IProvider<TQueryData>>();
        private string _defaultProvider = string.Empty;

        public Builder(string alias){
            _alias = alias;
            _config = new Config<TQueryData>.Builder().Build();
        }

        #region Configuration
        public Builder With(IConfig<TQueryData> config) {
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
        public Builder RegisterProvider(IProvider<TQueryData> provider, bool isDefault = false){
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

    private readonly IConfig<TQueryData> _config;
    private readonly IReadOnlyDictionary<string, IProvider<TQueryData>> _dataProviders;
    private readonly string _defaultProvider;
    private readonly ILogger<SearchEngine<TQueryData>> _logger;

    private IEvaluator<TQueryData> _evaluator => _config.Evaluator;

    private SearchEngine(string alias, IConfig<TQueryData> config, 
        IReadOnlyDictionary<string, IProvider<TQueryData>> providers,
        string defaultProvider){
        Alias = alias;
        DataType = typeof(TQueryData);
        
        _config = config;
        _logger = _config.LoggerFactory.CreateLogger<SearchEngine<TQueryData>>();
        _dataProviders = providers;
        _defaultProvider = defaultProvider;
    }

    #region Sync
    public ISearchResult<TQueryData> Query(string query, string? dataProvider = null)
    {
        return QueryAsync(query, dataProvider).Await();
    }
    public ISearchResult<TQueryData> Query(Query query, string? dataProvider = null)
    {
        return QueryAsync(query, dataProvider).Await();
    }
    ISearchResult ISearchEngine.Query(string query, string? dataProvider)
    {
        var res = Query(query, dataProvider);
        return new SearchResult {
            Input = res.Input,
            Total =  res.Total,
            Content = res.Content.Cast<QueryData>().ToArray()
        };
    }
    ISearchResult ISearchEngine.Query(Query query, string? dataProvider)
    {
        var res = Query(query, dataProvider);
        return new SearchResult {
            Input = res.Input,
            Total =  res.Total,
            Content = res.Content.Cast<QueryData>().ToArray()
        };
    }
    #endregion


    #region Async
    async Task<ISearchResult> ISearchEngine.QueryAsync(string query, string? dataProvider, CancellationToken ct)
    {
        var result = await QueryAsync(query, dataProvider, ct);
        return new SearchResult {
            Input = result.Input,
            Total = result.Total,

            Content = result.Content.Cast<QueryData>().ToArray()
        };
    }
    async Task<ISearchResult> ISearchEngine.QueryAsync(Query query, string? dataProvider, CancellationToken ct)
    {
        var result = await QueryAsync(query, dataProvider, ct);
        return new SearchResult {
            Input = result.Input,
            Total = result.Total,

            Content = result.Content.Cast<QueryData>().ToArray()
        };
    }

    public Task<ISearchResult<TQueryData>> QueryAsync(string query, string? dataProvider = null, CancellationToken ct = default){
        var parseResult = QueryParser.Query.TryParse(query);
        if(!parseResult.WasSuccessful) throw new SearchExpception(parseResult.Message);
        var parsedQuery = parseResult.Value;

        return QueryAsync(parsedQuery, dataProvider, ct);
    }
    public async Task<ISearchResult<TQueryData>> QueryAsync(Query query, string? dataProvider = null, CancellationToken ct = default){
        Expression<Func<TQueryData, bool>>? queryExpression = null;
        
        try{
            ct.ThrowIfCancellationRequested();
            var targetProvider = query.Provider?.ProviderId ?? dataProvider ?? _defaultProvider;
            
            var foundProvider = _dataProviders.TryGetValue(targetProvider, out var provider);
            _logger.LogInformation("{Provider} [{Status}] -> {Query}",
                targetProvider,
                foundProvider ? "Found" : "Unknown",
                query);
            if(!foundProvider || provider == null) throw new SearchExpception($"Data provider \"{targetProvider}\" not registred");
            
            ct.ThrowIfCancellationRequested();

            if(query.Constraint.HasExpression){
                queryExpression = FromQuery(query);
                _logger.LogInformation("From query[{Query}] derived:\n{Expression}",
                    query, queryExpression.ToString());
            }

            ct.ThrowIfCancellationRequested();

            var result = await provider.GetAsync(query.CommandExpression.Commands, queryExpression, ct);

            return new SearchResult<TQueryData>{
                Input = new SearchInput {
                    Query = query.ToString(),
                    EvaluatedExpression = queryExpression?.ToString() ?? string.Empty
                },
                Total = result.Count,
                Content = result.Content
            };
        }
        catch(OperationCanceledException oce) {
            _logger.LogInformation(oce, "Operation was cancelled via cancellation token");
            return new SearchResult<TQueryData>{
                Input = new SearchInput {
                    Query = query.ToString(),
                    EvaluatedExpression = queryExpression?.ToString() ?? string.Empty
                },
                Total = 0,
                Content = Array.Empty<TQueryData>()
            };
        }
        catch(Exception exp){ 
            _logger.LogCritical(exp, "Unexpected error for query [{Query}]", query);
            throw new SearchExpception($"Unexpected error for query [{query}]", exp);
        }
    }
    #endregion

    #region Expression Composition
    private Expression<Func<TQueryData, bool>> FromQuery(Query query) {
        var queryExpression = query.Constraint.Root switch {
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
            ListDirective list => _evaluator.Evaluate(list),
            
            _ => throw new Exception($"Unexpected directive type: {exp.Directive.GetType().Name}")
        };

        return LinqExp.Lambda<Func<TQueryData, bool>>(lambdaExpression.Body, 
            $"Directive[{exp.Directive.Type}->{exp.Directive.Identifier}]", lambdaExpression.Parameters);
    }
    #endregion
}