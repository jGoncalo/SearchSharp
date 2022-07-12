using System.Linq.Expressions;

namespace SearchSharp.Engine.Evaluators.Visitor;

internal class ReplaceStringVisitor<TQueryData> : ExpressionVisitor 
    where TQueryData : class {

    private readonly string _value;
    private ParameterExpression _target = Expression.Parameter(typeof(object));

    public ReplaceStringVisitor(string value) {
        _value = value;
    }

    public Expression<Func<TQueryData, bool>> Replace(Expression<Func<TQueryData, string, bool>> expression)  
    {  
        _target = expression.Parameters.First(p => p.Type == typeof(string));
        var afterVisit = Visit(expression.Body);

        return Expression.Lambda<Func<TQueryData, bool>>(afterVisit!,
            expression.Parameters.First(p => p.Type == typeof(TQueryData)));
    }

    public override Expression? Visit(Expression? node)
    {
        var isTarget = node == _target;
        return isTarget ? 
            Expression.Constant(_value, _value.GetType()) : base.Visit(node);
    }
}