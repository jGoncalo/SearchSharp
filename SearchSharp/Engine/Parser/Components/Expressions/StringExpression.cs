namespace SearchSharp.Engine.Parser.Components.Expressions;

public class StringExpression : LogicExpression {
    public readonly string Value;

    public StringExpression(string value) : base(ExpType.String) {
        Value = value;
    }

    public override string ToString() => $"\"{Value.ToString()}\"";
}