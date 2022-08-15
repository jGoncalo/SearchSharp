namespace SearchSharp.Engine.Parser.Components.Expressions;

public record EmptyExpression : Expression {
    public EmptyExpression() : base(ExpType.None) {}

    public override string ToString() => string.Empty;
}