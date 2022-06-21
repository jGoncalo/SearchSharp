using System.Linq;
using System.Linq.Expressions;
using SearchSharp.Items;

namespace SearchSharp.Engine.Rules.Visitor;

public class RuleExpressionVisitor<TQueryData, TLiteral> : ExpressionVisitor 
    where TQueryData : class 
    where TLiteral : Literal {

    private readonly TLiteral _literal;

    public RuleExpressionVisitor(TLiteral literal) {
        _literal = literal;
    }

    public Expression<Func<TQueryData, bool>> EvaluateLiterals(Expression<Func<TQueryData, TLiteral, bool>> expression)  
    {  
        var afterVisit = Visit(expression) as Expression<Func<TQueryData, TLiteral, bool>>;

        return Expression.Lambda<Func<TQueryData, bool>>(afterVisit!.Body,
            afterVisit.Parameters.Where(p => p.Type == typeof(TQueryData)).First());
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if(node.Member.DeclaringType == typeof(TLiteral)){
            return ReplaceNumericLiteral(node);
        }
        if(node.Member.DeclaringType == typeof(TQueryData)){
            return AssureQueryData(node);
        }

        return base.VisitMember(node);
    }

    private Expression AssureQueryData(MemberExpression member){
        /* TODO: assure TQueryData parameter for expression is named the same in all expressions
         *       and all parameters are mapped to the same ParameterExpression (created in visitor)
         *       ex: "(some) => some.Value" is swapped to "(data) => data.Value"
        */
        return member;
    }

    private Expression ReplaceNumericLiteral(MemberExpression member){
        var parameter = member.Expression as ParameterExpression;
        var objMember = Expression.Convert(member, typeof(object));
        var lambda = Expression.Lambda<Func<TLiteral, object>>(objMember, parameter!);

        var result = lambda.Compile()(_literal);

        return Expression.Constant(result, result.GetType());
    }
}