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
        _target = expression.Parameters.Where(p => p.Type == typeof(string)).First()!;
        var afterVisit = Visit(expression.Body);

        return Expression.Lambda<Func<TQueryData, bool>>(afterVisit!,
            expression.Parameters.Where(p => p.Type == typeof(TQueryData)).First());
    }

    public override Expression? Visit(Expression? node)
    {
        var isTarget = node == _target;
        return isTarget ? 
            Expression.Constant(_value, _value.GetType()) : base.Visit(node);
    }
}