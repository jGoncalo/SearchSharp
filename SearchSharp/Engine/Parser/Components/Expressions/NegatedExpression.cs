namespace SearchSharp.Engine.Parser.Components.Expressions;

public class NegatedExpression : LogicExpression {
    public readonly LogicExpression Negated;

    public NegatedExpression(LogicExpression negated) : base(ExpType.Negated) {
        Negated = negated;
    }

    public override string ToString() => $"!({Negated.ToString()})";
}