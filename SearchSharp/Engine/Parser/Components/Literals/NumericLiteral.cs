namespace SearchSharp.Engine.Parser.Components.Literals;

public record NumericLiteral : Literal {

    public int AsInt { get; }
    public float AsFloat { get; }

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

    public static NumericLiteral Float(string rawValue) => new NumericLiteral(rawValue, true);
    public static NumericLiteral Float(float value) => new NumericLiteral(value);
    
    public static NumericLiteral Int(string rawValue) => new NumericLiteral(rawValue, false);
    public static NumericLiteral Int(int value) => new NumericLiteral(value);

    public static NumericLiteral Min => new NumericLiteral(true);
    public static NumericLiteral Max => new NumericLiteral(false);

    public override string ToString() => RawValue;
}