namespace SearchSharp.Engine.Parser.Components.Literals;

/// <summary>
/// DQL String Literal
/// </summary>
/// <param name="Value">string value</param>
public record StringLiteral(string Value) : Literal(Value, LiteralType.String) {
    /// <summary>
    /// Cast value as enumerable
    /// </summary>
    /// <typeparam name="TEnum">Type of enumerable for conversion</typeparam>
    /// <returns>Enumerable value</returns>
    public TEnum AsEnum<TEnum>() where TEnum : Enum {
        var couldParse = Enum.TryParse(typeof(TEnum), Value, true, out var @enum);
        if (couldParse && @enum is TEnum val) {
            return val;
        }
        return default!;
    }

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => $"\"{RawValue.ToString()}\"";
}
