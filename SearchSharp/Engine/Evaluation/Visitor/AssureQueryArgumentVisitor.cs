using System.Linq.Expressions;

namespace SearchSharp.Engine.Evaluation.Visitor;

internal class AssureQueryArgumentVisitor<TQueryData> : ExpressionVisitor 
    where TQueryData : QueryData {
    private readonly ParameterExpression _dataParameter = Expression.Parameter(typeof(TQueryData), "v_data");

    public AssureQueryArgumentVisitor(){

    }
    
    public Expression<Func<TQueryData, bool>> Assure(Expression<Func<TQueryData, bool>> expression)  
    {  
        return (Visit(expression) as Expression<Func<TQueryData, bool>>)!;
    }


    protected override Expression VisitParameter(ParameterExpression node)
    {
        if(node.Type == typeof(TQueryData)){
            return _dataParameter;
        }

        return base.VisitParameter(node);
    }
}