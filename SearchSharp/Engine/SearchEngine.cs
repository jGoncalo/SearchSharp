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
    private readonly ISearchEngine<TQueryData>.IConfig _config;
    private readonly ISearchEngine<TQueryData>.IEvaluator _evaluator;
    private readonly Dictionary<string, ISearchEngine<TQueryData>.IDataProvider> _dataProviders = new();
    private readonly ILogger<SearchEngine<TQueryData>> _logger;

    public SearchEngine(ISearchEngine<TQueryData>.IConfig config){
        _config = config;
        _evaluator = new Evaluator<TQueryData>(_config);
        _logger = _config.LoggerFactory.CreateLogger<SearchEngine<TQueryData>>();
    }

    #region Providers
    public SearchEngine<TQueryData> RegisterProvider(ISearchEngine<TQueryData>.IDataProvider provider){
        _dataProviders[provider.Name] = provider;
        return this;
    }
    public SearchEngine<TQueryData> RemoveProvider(ISearchEngine<TQueryData>.IDataProvider provider) {
        return RemoveProvider(provider.Name);
    }
    public SearchEngine<TQueryData> RemoveProvider(string providerName) {
        var contains = _dataProviders.ContainsKey(providerName);
        if(contains) _dataProviders.Remove(providerName);
        return this;
    }
    #endregion

    public IQueryable<TQueryData> Query(string dataProvider, string query){
        if(string.IsNullOrWhiteSpace(dataProvider)) throw new ArgumentException("Null or empty argument", nameof(dataProvider));
        if(string.IsNullOrWhiteSpace(query)) throw new ArgumentException("Null or empty argument", nameof(query));

        var foundProvider = _dataProviders.TryGetValue(dataProvider, out var provider);
        _logger.LogInformation("{Provider} [{Status}] -> {Query}",
            dataProvider,
            foundProvider ? "Found" : "Unknown",
            query);
        if(!foundProvider || provider == null) throw new Exception($"Data provider \"{dataProvider}\" not registred");
        try{
            var queryLambda = FromQuery(query);
            _logger.LogInformation("From query[{Query}] derived:\n{Expression}",
                query, queryLambda.ToString());
            var providerDataSource = provider.DataSource();
            return providerDataSource.Where(queryLambda);
        }
        catch(Exception exp){ 
            _logger.LogCritical(exp, "Unexpected error for query [{Query}]", query);
            throw new SearchExpception($"Unexpected error for query [{query}]", exp);
        }
    }

    private Expression<Func<TQueryData, bool>> FromQuery(string query) {
        var result = QueryParser.Query.TryParse(query);
        if(!result.WasSuccessful) throw new Exception(result.Message);

        var queryExpression = result.Value.Root switch {
            LogicExpression compute => FromExpression(compute),
            StringExpression @string => FromExpression(@string),

            _ => throw new Exception($"Unexpected query expression type: {result.Value.Root.GetType().Name}")
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