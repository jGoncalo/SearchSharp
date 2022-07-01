namespace SearchSharp.Engine.Parser.Components.Expressions;

public class StringExpression : Expression {
    public readonly string Value;

    public StringExpression(string value) : base(ExpType.String) {
        Value = value;
    }
}