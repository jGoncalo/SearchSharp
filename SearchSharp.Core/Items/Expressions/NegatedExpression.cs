namespace SearchSharp.Core.Items.Expressions;

public class NegatedExpression : LogicExpression {
    public readonly LogicExpression Negated;

    public NegatedExpression(LogicExpression negated) : base(ExpressionType.Negated) {
        Negated = negated;
    }
}