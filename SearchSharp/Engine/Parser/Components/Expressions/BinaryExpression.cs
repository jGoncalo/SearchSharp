namespace SearchSharp.Engine.Parser.Components.Expressions;

public class BinaryExpression : LogicExpression {
    public readonly LogicOperator Operator;
    public readonly LogicExpression Left;
    public readonly LogicExpression Right;

    public BinaryExpression(LogicOperator logicOperator, LogicExpression left, LogicExpression right) : base(ExpType.Logic) {
        Operator = logicOperator;
        Left = left;
        Right = right;
    }

    public override string ToString(){
        var opStr = Operator switch {
            LogicOperator.And => "&",
            LogicOperator.Or => "|",
            LogicOperator.Xor => "^",

            _ => Operator.ToString()
        };

        return $"({Left.ToString()} {opStr} {Right.ToString()})";
    }
}