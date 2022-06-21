namespace SearchSharp.Items.Expressions;

public class NegatedExpression : LogicExpression {
    public readonly LogicExpression Negated;

    public NegatedExpression(LogicExpression negated) : base(ExpType.Negated) {
        Negated = negated;
    }
}