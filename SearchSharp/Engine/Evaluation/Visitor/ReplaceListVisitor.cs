using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Evaluation.Visitor;

internal class ReplaceListVisitor<TQueryData, TLiteral> : ExpressionVisitor 
    where TQueryData : QueryData
    where TLiteral : Literal {

    private readonly TLiteral[] _arguments;
    private ParameterExpression _argumentParameter = Expression.Parameter(typeof(object));

    public ReplaceListVisitor(TLiteral[] arguments) {
        _arguments = arguments;
    }

    public Expression<Func<TQueryData, bool>> Replace(Expression<Func<TQueryData, TLiteral[], bool>> expression)  
    {  
        _argumentParameter = expression.Parameters.Where(p => p.Type == typeof(TLiteral[])).First();
        var queryDataParameter = expression.Parameters.Where(p => p.Type == typeof(TQueryData)).First();

        var afterVisit = Visit(expression) as Expression<Func<TQueryData, TLiteral[], bool>>;
        return Expression.Lambda<Func<TQueryData, bool>>(afterVisit!.Body,
            queryDataParameter);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var newArgList = new List<Expression>(node.Arguments.Count());
        var wasReplaced = false;

        foreach(var arg in node.Arguments){
            var isReplacable = arg.NodeType == ExpressionType.Parameter && arg.Type == typeof(TLiteral[]);
            
            wasReplaced |= isReplacable;

            if(!isReplacable) newArgList.Add(arg);
            else {
                var newLitArgument = ReplaceListLiteral((ParameterExpression) arg);
                newArgList.Add(newLitArgument);
            }
        }

        return wasReplaced? Expression.Call(node.Method, newArgList) : base.VisitMethodCall(node);
    }

    private Expression ReplaceListLiteral(ParameterExpression member){
        var arrExpr = Expression.NewArrayInit(typeof(TLiteral), 
            _arguments.Select(arg => Expression.Constant(arg)));

        return arrExpr;
    }

    
}