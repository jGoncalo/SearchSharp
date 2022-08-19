using SearchSharp.Exceptions;
using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Converters;

public class DefaultConverter : IConverter
{
    public virtual object From(Type type, Literal literal){
        switch (literal) {
            case StringLiteral str:
                return From(type, str);
            case NumericLiteral num:
                return From(type, num);
            case BooleanLiteral @bool:
                return From(type, @bool);
            default:
                var msg = string.Join(",", Enum.GetValues<LiteralType>());
                throw new ArgumentException($"Literal should have type: [{msg}]", nameof(literal));
        }
    }
    public virtual object From(Type type, StringLiteral stringLiteral) {
        if(type == typeof(string)) return (object) stringLiteral.Value;
        if(type.IsEnum) {
            var parsed = Enum.TryParse(type, stringLiteral.Value, true, out var @enum);
            if(parsed && @enum != null) return @enum;
            else return (object) 0;
        }

        throw new ConvertionException(LiteralType.String, type);
    }
    public virtual object From(Type type, NumericLiteral numericLiteral) {
        if(type == typeof(string)){
            return (object) numericLiteral.RawValue;
        }
        if(type == typeof(int)) return (object) numericLiteral.AsInt;
        if(type == typeof(float)) return (object) numericLiteral.AsFloat;
        if(type.IsEnum){
            return (object) numericLiteral.AsInt;
        }

        throw new ConvertionException(LiteralType.Numeric, type);
    }
    public virtual object From(Type type, BooleanLiteral booleanLiteral) {
        if(type == typeof(string)){
            return (object) booleanLiteral.RawValue;
        }
        if(type == typeof(int)) return (object) Convert.ToInt32(booleanLiteral.Value);
        if(type == typeof(float)) return (object) Convert.ToInt32(booleanLiteral.Value);

        throw new ConvertionException(LiteralType.Boolean, type);
    }

    public TType From<TType>(Literal literal) => (TType) From(typeof(TType), literal);
    public TType From<TType>(StringLiteral stringLiteral) => (TType) From(typeof(TType), stringLiteral);
    public TType From<TType>(NumericLiteral numericLiteral) => (TType) From(typeof(TType), numericLiteral);
    public TType From<TType>(BooleanLiteral booleanLiteral) => (TType) From(typeof(TType), booleanLiteral);
}