namespace SearchSharp.Engine.Parser.Components.Literals;

public record BooleanLiteral(bool Value) : Literal(Value.ToString(), LiteralType.Boolean) {
    public override string ToString() => RawValue;
}
