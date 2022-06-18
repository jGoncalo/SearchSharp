namespace SearchSharp.Core.Items.Expressions;

public abstract class NegatedExpression : ComputeExpression {
    public readonly ComputeExpression Negated;

    public NegatedExpression(ComputeExpression negated) : base(ExpressionType.Negated) {
        Negated = negated;
    }
}