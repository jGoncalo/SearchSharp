namespace SearchSharp.Engine.Parser.Components.Literals;

/// <summary>
/// DQL Boolean Literal
/// </summary>
/// <param name="Value">Value of the boolean</param>
public record BooleanLiteral(bool Value) : Literal(Value.ToString(), LiteralType.Boolean) {
    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => RawValue;
}
