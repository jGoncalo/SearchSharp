namespace SearchSharp.Engine.Parser.Components.Literals;

/// <summary>
/// DQL Numeric Literal
/// </summary>
public record NumericLiteral : Literal {

    /// <summary>
    /// Cast value as Integer
    /// </summary>
    public int AsInt { get; }
    /// <summary>
    /// Cast value as Float
    /// </summary>
    public float AsFloat { get; }
    /// <summary>
    /// Cast value as Enumerable
    /// </summary>
    /// <typeparam name="TEnum">Type of enumerable for conversion</typeparam>
    /// <returns>Enumerable value</returns>
    public TEnum AsEnum<TEnum>() where TEnum : Enum {
        return (TEnum) (object) AsInt;
    }

    private NumericLiteral(string rawValue, bool isFloat) : base(rawValue, LiteralType.Numeric) {
        if(isFloat) {
            AsFloat = float.TryParse(RawValue, out var floatValue) ? floatValue : 0.0f;
            AsInt = (int) AsFloat;
        }
        else {
            AsInt = int.TryParse(RawValue, out var intValue) ? intValue : 0;
            AsFloat = (float) AsInt;
        }
    }
    private NumericLiteral(int value) : base(value.ToString(), LiteralType.Numeric) {
        AsInt = value;
        AsFloat = value;
    }
    private NumericLiteral(float value) : base(value.ToString(), LiteralType.Numeric) {
        AsInt = (int) value;
        AsFloat = value;
    }
    private NumericLiteral(bool isMin) : base(string.Empty, LiteralType.Numeric) {
        AsInt = isMin ? int.MinValue : int.MaxValue;
        AsFloat = isMin ? float.MinValue : float.MaxValue;
    }

    /// <summary>
    /// Instance for a float value
    /// </summary>
    /// <param name="rawValue">string value</param>
    /// <returns>DQL Numeric Literal</returns>
    public static NumericLiteral Float(string rawValue) => new NumericLiteral(rawValue, true);
    /// <summary>
    /// Instance for a float value
    /// </summary>
    /// <param name="value">float value</param>
    /// <returns>DQL Numeric Literal</returns>
    public static NumericLiteral Float(float value) => new NumericLiteral(value);
    
    /// <summary>
    /// Instance for a integer value
    /// </summary>
    /// <param name="rawValue">string value</param>
    /// <returns>DQL Numeric Literal</returns>
    public static NumericLiteral Int(string rawValue) => new NumericLiteral(rawValue, false);
    /// <summary>
    /// Instance for a integer value
    /// </summary>
    /// <param name="value">integer value</param>
    /// <returns>DQL Numeric Literal</returns>
    public static NumericLiteral Int(int value) => new NumericLiteral(value);

    /// <summary>
    /// Instance of a DQL Numeric literal with minimum value
    /// </summary>
    public static NumericLiteral Min => new NumericLiteral(true);
    /// <summary>
    /// Instance of a DQL Numeric Literal with maximum value
    /// </summary>
    public static NumericLiteral Max => new NumericLiteral(false);

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => RawValue;
}