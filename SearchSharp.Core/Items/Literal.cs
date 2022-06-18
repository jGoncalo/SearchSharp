namespace SearchSharp.Core.Items;

public abstract class Literal : Item {
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

    public readonly int AsInt;
    public readonly float AsFloat;

    public NumericLiteral(string rawValue, bool isFloat) : base(LiteralType.Numeric, rawValue) {
        if(isFloat) {
            AsFloat = float.TryParse(RawValue, out var floatValue) ? floatValue : 0.0f;
            AsInt = (int) AsFloat;
        }
        else {
            AsInt = int.TryParse(RawValue, out var intValue) ? intValue : 0;
            AsFloat = (float) AsInt;
        }
        
    }
}