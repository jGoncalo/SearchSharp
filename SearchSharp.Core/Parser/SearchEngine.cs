namespace SearchSharp.Core.Parser;

using System.Collections.Generic;
using System.Linq.Expressions;
using SearchSharp.Core.Items;
using BinExpression = SearchSharp.Core.Items.BinaryExpression;

public class SearchEngine<TQueryData> 
    where TQueryData : class {
    public interface IAnalyzer {
        Expression<Func<TQueryData, bool>> AnalyzeDirective(QueryDirective directive);
        Expression<Func<TQueryData, bool>> AnalyzeLiteral(QueryLiteral literal);
    }
    public interface IDataProvider {
        string Name { get; }

        Task<IQueryable<TQueryData>> DataSourceAsync(CancellationToken ct = default);
        IQueryable<TQueryData> DataSource();
    }

    private readonly IAnalyzer _analyzer;
    private readonly Dictionary<string, IDataProvider> _dataProviders = new();

    public SearchEngine(IAnalyzer analyzer){
        _analyzer = analyzer;
    }

    #region Providers
    public SearchEngine<TQueryData> RegisterProvider(IDataProvider provider){
        _dataProviders[provider.Name] = provider;
        return this;
    }
    public SearchEngine<TQueryData> RemoveProvider(IDataProvider provider) {
        return RemoveProvider(provider.Name);
    }
    public SearchEngine<TQueryData> RemoveProvider(string providerName) {
        var contains = _dataProviders.ContainsKey(providerName);
        if(contains) _dataProviders.Remove(providerName);
        return this;
    }
    #endregion

    public IQueryable<TQueryData> Query(string dataProvider, string query){
        var foundProvider = _dataProviders.TryGetValue(dataProvider, out var provider);
        if(!foundProvider || provider == null) throw new Exception($"Data provider \"{dataProvider}\" not registred");

        try{
            var queryLambda = QueryToExpression(query);
            return provider.DataSource().Where(queryLambda);
        }
        catch(Exception exp){
            //TODO: add info to thrown
            throw;
        }
    }

    private Expression<Func<TQueryData, bool>> QueryToExpression(string query) {
        var couldParse = QueryParser.TryParse(query, out var errReason, out var parsedQuery);
        if(!couldParse) throw new Exception(errReason);

        if(parsedQuery is QueryLiteral literal){
            return _analyzer.AnalyzeLiteral(literal);
        }
        else if(parsedQuery is QueryExpression expression) {
            return FromExpression(expression);
        }
        else throw new Exception($"unexpected parsedQuery type: {parsedQuery.GetType().Name}");
    }

    private Expression<Func<TQueryData, bool>> FromExpression(QueryExpression exp){
        return exp switch {
            DirectiveExpression dir => FromDirectiveExpression(dir),
            NegateExpression neg => FromNegativeExpression(neg),
            BinExpression bin => FromBinaryExpression(bin),
            QueryExpression => FromExpression(exp),

            _ => throw new Exception($"Unexpected query expression type: {exp.GetType().Name}")
        };
    }
    private Expression<Func<TQueryData, bool>> FromBinaryExpression(BinExpression exp){
        var leftLambda = FromExpression(exp.Left);
        var rightLambda = FromExpression(exp.Right);

        Expression composedLambda = exp.OpType switch 
        {
            BinaryOperationType.Or => Expression.Or(leftLambda, rightLambda),
            BinaryOperationType.And => Expression.Add(leftLambda, rightLambda),
            _ => throw new Exception($"unexpected OperationType value: {exp.OpType}")
        };

        var argExpression = Expression.Parameter(typeof(TQueryData));
        return Expression.Lambda<Func<TQueryData, bool>>(composedLambda, 
            $"{leftLambda.Name}|{rightLambda.Name}", new [] {argExpression});
    }
    private Expression<Func<TQueryData, bool>> FromNegativeExpression(NegateExpression exp){
        if(exp.Child == null) throw new Exception("Unexpected negative expression with null child");
        var lambdaExpression = FromExpression(exp.Child);

        var composedLambda = Expression.Not(lambdaExpression);
        var argExpression = Expression.Parameter(typeof(TQueryData));

        return Expression.Lambda<Func<TQueryData, bool>>(composedLambda,
            $"!{lambdaExpression.Name}", new [] {argExpression});
    }
    private Expression<Func<TQueryData, bool>> FromDirectiveExpression(DirectiveExpression exp){
        var lambdaExpression = _analyzer.AnalyzeDirective(exp.Directive);
        var argExpression = Expression.Parameter(typeof(TQueryData));

        return Expression.Lambda<Func<TQueryData, bool>>(lambdaExpression, 
            $"dir[{exp.Directive.Identifier}]=>{exp.Directive.Value}", new [] {argExpression});
    }
}