namespace SearchSharp.Core.Items.Expressions;

public class StringExpression : Expression {
    public readonly string Value;

    public StringExpression(string value) : base(ExpressionType.String) {
        Value = value;
    }
}