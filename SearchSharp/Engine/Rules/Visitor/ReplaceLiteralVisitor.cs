using System.Linq.Expressions;
using SearchSharp.Items;

namespace SearchSharp.Engine.Rules.Visitor;

public class ReplaceLiteralVisitor<TQueryData, TLiteral> : ExpressionVisitor 
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
            afterVisit.Parameters.Where(p => p.Type == typeof(TQueryData)).First());
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if(node.Member.DeclaringType == typeof(TLiteral)){
            return ReplaceLiteral(node);
        }

        return base.VisitMember(node);
    }

    private Expression ReplaceLiteral(MemberExpression member){
        var parameter = member.Expression as ParameterExpression;
        var objMember = Expression.Convert(member, typeof(object));
        var lambda = Expression.Lambda<Func<TLiteral, object>>(objMember, parameter!);

        var result = lambda.Compile()(_literal);

        return Expression.Constant(result, result.GetType());
    }
}