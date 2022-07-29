namespace SearchSharp.Engine.Parser.Components.Expressions;

public abstract class LogicExpression : Expression {
    public LogicExpression(ExpType type) : base(type) {
        
    }

    public NegatedExpression Negate() => new NegatedExpression(this);

    public static NegatedExpression operator !(LogicExpression exp) => exp.Negate();
    public static BinaryExpression operator &(LogicExpression left, LogicExpression right) => new BinaryExpression(LogicOperator.And, left, right);
    public static BinaryExpression operator |(LogicExpression left, LogicExpression right) => new BinaryExpression(LogicOperator.Or, left, right);
    public static BinaryExpression operator ^(LogicExpression left, LogicExpression right) => new BinaryExpression(LogicOperator.Xor, left, right);
}