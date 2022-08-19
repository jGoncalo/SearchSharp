namespace SearchSharp.Engine.Parser.Components.Literals;

public abstract record Literal(string RawValue, LiteralType Type) : QueryItem{
    public static LiteralType? TypeOf(Type type) {
        if(type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return LiteralType.Numeric;
        if(type == typeof(string)) return LiteralType.String;
        if(type == typeof(bool)) return LiteralType.Boolean;
        if(type.IsEnum) return LiteralType.String;

        return null;
    }
}
