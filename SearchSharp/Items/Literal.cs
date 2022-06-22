namespace SearchSharp.Items;

public abstract class Literal : QueryItem {
    public readonly string RawValue;
    public readonly LiteralType Type;

    public Literal(LiteralType type, string rawValue) {
        Type = type;
        RawValue = rawValue;
    }
}

public class StringLiteral : Literal {
    public readonly string Value;

    public StringLiteral(string value) : base(LiteralType.String, value) {
        Value = value;
    }
}

public class NumericLiteral : Literal {

    public int AsInt { get; }
    public float AsFloat { get; }

    private NumericLiteral(string rawValue, bool isFloat) : base(LiteralType.Numeric, rawValue) {
        if(isFloat) {
            AsFloat = float.TryParse(RawValue, out var floatValue) ? floatValue : 0.0f;
            AsInt = (int) AsFloat;
        }
        else {
            AsInt = int.TryParse(RawValue, out var intValue) ? intValue : 0;
            AsFloat = (float) AsInt;
        }
        
    }
    private NumericLiteral(bool isMin) : base(LiteralType.Numeric, string.Empty) {
        AsInt = isMin ? int.MinValue : int.MaxValue;
        AsFloat = isMin ? float.MinValue : float.MaxValue;
    }

    public static NumericLiteral Float(string rawValue) => new NumericLiteral(rawValue, true);
    public static NumericLiteral Int(string rawValue) => new NumericLiteral(rawValue, false);

    public static NumericLiteral Min => new NumericLiteral(true);
    public static NumericLiteral Max => new NumericLiteral(false);
}