namespace SearchSharp.Items.Expressions;

public class BinaryExpression : LogicExpression {
    public readonly LogicOperator Operator;
    public readonly LogicExpression Left;
    public readonly LogicExpression Right;

    public BinaryExpression(LogicOperator logicOperator, LogicExpression left, LogicExpression right) : base(ExpressionType.Logic) {
        Operator = logicOperator;
        Left = left;
        Right = right;
    }
}