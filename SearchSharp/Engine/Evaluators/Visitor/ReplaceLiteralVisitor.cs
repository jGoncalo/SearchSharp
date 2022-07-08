using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Evaluators.Visitor;

internal class ReplaceLiteralVisitor<TQueryData, TLiteral> : ExpressionVisitor 
    where TQueryData : class 
    where TLiteral : Literal {

    private readonly TLiteral _literal;

    public ReplaceLiteralVisitor(TLiteral literal) {
        _literal = literal;
    }

    public Expression<Func<TQueryData, bool>> Replace(Expression<Func<TQueryData, TLiteral, bool>> expression)  
    {  
        var afterVisit = Visit(expression) as Expression<Func<TQueryData, TLiteral, bool>>;

        return Expression.Lambda<Func<TQueryData, bool>>(afterVisit!.Body,
            afterVisit.Parameters.First(p => p.Type == typeof(TQueryData)));
    }

    protected override Expression VisitMember(MemberExpression node) {
        return node.Member.DeclaringType == typeof(TLiteral) ? 
            ReplaceLiteral(node) : base.VisitMember(node);
    }

    private Expression ReplaceLiteral(MemberExpression member){
        var parameter = member.Expression as ParameterExpression;
        var objMember = Expression.Convert(member, typeof(object));
        var lambda = Expression.Lambda<Func<TLiteral, object>>(objMember, parameter!);

        var result = lambda.Compile()(_literal);

        return Expression.Constant(result, result.GetType());
    }
}