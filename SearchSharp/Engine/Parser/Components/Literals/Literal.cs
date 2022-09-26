namespace SearchSharp.Engine.Parser.Components.Literals;

/// <summary>
/// Abstract DQL Literal
/// </summary>
/// <param name="RawValue">String value</param>
/// <param name="Type">Type of DQL literal</param>
public abstract record Literal(string RawValue, LiteralType Type) : QueryItem{
    /// <summary>
    /// Obtain DQL literal type from C# type
    /// </summary>
    /// <param name="type">C# type to be matched to DQL type</param>
    /// <returns>DQL Literal Type covering C# type, if any exists</returns>
    public static LiteralType? TypeOf(Type type) {
        if(type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return LiteralType.Numeric;
        if(type == typeof(string)) return LiteralType.String;
        if(type == typeof(bool)) return LiteralType.Boolean;
        if(type.IsEnum) return LiteralType.String;

        return null;
    }
}
