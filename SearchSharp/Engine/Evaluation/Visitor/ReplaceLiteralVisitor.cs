using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Evaluation.Visitor;

internal class ReplaceLiteralVisitor<TQueryData, TLiteral> : ExpressionVisitor 
    where TQueryData : QueryData 
    where TLiteral : Literal {

    private readonly TLiteral _literal;

    public ReplaceLiteralVisitor(TLiteral literal) {
        _literal = literal;
    }

    private ParameterExpression _dataArgument = Expression.Parameter(typeof(object));
    private ParameterExpression _litArgument = Expression.Parameter(typeof(object));

    public Expression<Func<TQueryData, bool>> Replace(Expression<Func<TQueryData, TLiteral, bool>> expression) {
        _dataArgument = expression.Parameters.First(p => p.Type == typeof(TQueryData));
        _litArgument = expression.Parameters.First(p => p.Type == typeof(TLiteral));
        
        var afterVisit = Visit(expression) as Expression<Func<TQueryData, TLiteral, bool>>;

        return Expression.Lambda<Func<TQueryData, bool>>(afterVisit!.Body,
            afterVisit.Parameters.First(p => p.Type == typeof(TQueryData)));
    }

    protected override Expression VisitMember(MemberExpression node) {
        return node.Member.DeclaringType == typeof(TLiteral) ? 
            ReplaceLiteral(node) : base.VisitMember(node);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node) {
        return node.Method.DeclaringType == typeof(TLiteral) ? 
            ReplaceLiteral(node) : base.VisitMethodCall(node);
    }

    private Expression ReplaceLiteral(MethodCallExpression call) {
        var objCall = Expression.Convert(call, typeof(object));
        var lambda = Expression.Lambda<Func<TLiteral, object>>(objCall, _litArgument);

        var result = lambda.Compile()(_literal);

        return Expression.Constant(result, result.GetType());
    }
    
    private Expression ReplaceLiteral(MemberExpression member){
        var parameter = member.Expression as ParameterExpression;
        var objMember = Expression.Convert(member, typeof(object));
        var lambda = Expression.Lambda<Func<TLiteral, object>>(objMember, parameter!);

        var result = lambda.Compile()(_literal);

        return Expression.Constant(result, result.GetType());
    }
}