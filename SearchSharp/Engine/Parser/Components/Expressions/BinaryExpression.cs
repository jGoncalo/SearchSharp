namespace SearchSharp.Engine.Parser.Components.Expressions;

public record BinaryExpression(LogicOperator Operator, LogicExpression Left, LogicExpression Right) : LogicExpression(ExpType.Logic) {

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