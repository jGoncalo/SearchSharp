namespace SearchSharp.Core.Items.Expressions;

public abstract class LogicExpression : ComputeExpression {
    public readonly LogicOperator Operator;
    public readonly ComputeExpression Left;
    public readonly ComputeExpression Right;

    public LogicExpression(LogicOperator logicOperator, ComputeExpression left, ComputeExpression right) : base(ExpressionType.Logic) {
        Operator = logicOperator;
        Left = left;
        Right = right;
    }
}