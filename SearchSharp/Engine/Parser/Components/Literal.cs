namespace SearchSharp.Engine.Parser.Components;

public abstract class Literal : QueryItem {
    public readonly string RawValue;
    public readonly LiteralType Type;

    public Literal(LiteralType type, string rawValue) {
        Type = type;
        RawValue = rawValue;
    }
}

public class BooleanLiteral : Literal {
    public readonly bool Value;

    public BooleanLiteral(bool value) : base(LiteralType.Boolean, value.ToString()) {
        Value = value;
    }

    public override string ToString() => RawValue;
}

public class StringLiteral : Literal {
    public readonly string Value;

    public TEnum AsEnum<TEnum>() where TEnum : Enum {
        var couldParse = Enum.TryParse(typeof(TEnum), Value, true, out var @enum);
        if (couldParse && @enum is TEnum val) {
            return val;
        }
        return default!;
    }

    public StringLiteral(string value) : base(LiteralType.String, value) {
        Value = value;
    }

    public override string ToString() => $"\"{RawValue.ToString()}\"";
}

public class NumericLiteral : Literal {

    public int AsInt { get; }
    public float AsFloat { get; }

    public TEnum AsEnum<TEnum>() where TEnum : Enum {
        return (TEnum) (object) AsInt;
    }

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
    private NumericLiteral(int value) : base(LiteralType.Numeric, value.ToString()) {
        AsInt = value;
        AsFloat = value;
    }
    private NumericLiteral(float value) : base(LiteralType.Numeric, value.ToString()) {
        AsInt = (int) value;
        AsFloat = value;
    }
    private NumericLiteral(bool isMin) : base(LiteralType.Numeric, string.Empty) {
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