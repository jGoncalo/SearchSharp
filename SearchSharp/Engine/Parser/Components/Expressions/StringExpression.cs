namespace SearchSharp.Engine.Parser.Components.Expressions;

public record StringExpression(string Value) : LogicExpression(ExpType.String) {
    public static StringExpression Empty => new StringExpression(string.Empty);

    public override string ToString() => $"\"{Value.ToString()}\"";
}