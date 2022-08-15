namespace SearchSharp.Engine.Parser.Components.Expressions;

public record NegatedExpression(LogicExpression Negated) : LogicExpression(ExpType.Negated) {
    public override string ToString() => $"!({Negated.ToString()})";
}