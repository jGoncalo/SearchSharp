namespace SearchSharp.Items.Expressions;

public class StringExpression : Expression {
    public readonly string Value;

    public StringExpression(string value) : base(ExpType.String) {
        Value = value;
    }
}