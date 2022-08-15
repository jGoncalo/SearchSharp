namespace SearchSharp.Engine.Parser.Components.Literals;

public record StringLiteral(string Value) : Literal(Value, LiteralType.String) {
    public TEnum AsEnum<TEnum>() where TEnum : Enum {
        var couldParse = Enum.TryParse(typeof(TEnum), Value, true, out var @enum);
        if (couldParse && @enum is TEnum val) {
            return val;
        }
        return default!;
    }

    public override string ToString() => $"\"{RawValue.ToString()}\"";
}
