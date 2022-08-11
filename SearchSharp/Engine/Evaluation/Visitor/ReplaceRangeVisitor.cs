using System.Linq.Expressions;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Evaluation.Visitor;

internal class ReplaceRangeVisitor<TQueryData> : ExpressionVisitor 
    where TQueryData : QueryData {

    private readonly NumericLiteral _lowerLiteral;
    private readonly NumericLiteral _upperLiteral;

    private ParameterExpression _lowerParameter = Expression.Parameter(typeof(object));
    private ParameterExpression _upperParameter = Expression.Parameter(typeof(object));

    public ReplaceRangeVisitor(NumericLiteral lowerLiteral, NumericLiteral upperLiteral) {
        _lowerLiteral = lowerLiteral;
        _upperLiteral = upperLiteral;
    }

    public Expression<Func<TQueryData, bool>> Replace(Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>> expression)  
    {  
        var arguments = expression.Parameters.Where(p => p.Type == typeof(NumericLiteral)).ToArray();
        _lowerParameter = arguments.First() as ParameterExpression;
        _upperParameter = arguments.Last() as ParameterExpression;

        var afterVisit = Visit(expression) as Expression<Func<TQueryData, NumericLiteral, NumericLiteral, bool>>;

        return Expression.Lambda<Func<TQueryData, bool>>(afterVisit!.Body,
            afterVisit.Parameters.Where(p => p.Type == typeof(TQueryData)));
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if(node.Member.DeclaringType == typeof(NumericLiteral)){
            return ReplaceLiteral(node);
        }

        return base.VisitMember(node);
    }
    
    private Expression ReplaceLiteral(MemberExpression member){
        var memberParameterName = (member.Expression as ParameterExpression)!.Name;
        NumericLiteral value;

        if(memberParameterName == _lowerParameter.Name){
            value = _lowerLiteral;
        }
        else if (memberParameterName == _upperParameter.Name) {
            value = _upperLiteral;
        }
        else return member;
        
        var parameter = member.Expression as ParameterExpression;
        var objMember = Expression.Convert(member, typeof(object));
        var lambda = Expression.Lambda<Func<NumericLiteral, object>>(objMember, parameter!);

        var result = lambda.Compile()(value);
        return Expression.Constant(result, result.GetType());
    }
}