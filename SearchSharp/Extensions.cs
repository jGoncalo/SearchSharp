using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp;

public static class Extensions {
    #region Literal    
    public static NumericLiteral AsLiteral(this int value) => NumericLiteral.Int(value);
    public static NumericLiteral AsLiteral(this float value) => NumericLiteral.Float(value);

    public static NumericLiteral AsIntLiteral(this string value) => NumericLiteral.Int(value);
    public static NumericLiteral AsFloatLiteral(this string value) => NumericLiteral.Float(value);
    
    public static StringLiteral AsLiteral(this string value) => new StringLiteral(value);
    public static StringLiteral AsLiteral<TEnum>(this TEnum value) where TEnum : Enum => new StringLiteral(value.ToString());

    public static BooleanLiteral AsLiteral(this bool value) => new BooleanLiteral(value);
    #endregion
}