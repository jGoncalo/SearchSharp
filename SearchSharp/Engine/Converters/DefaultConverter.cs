using SearchSharp.Exceptions;
using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Converters;

/// <summary>
/// C#/Literal type converter
/// </summary>
public class DefaultConverter : IConverter
{
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <param name="type">C# type</param>
    /// <param name="literal">Literal</param>
    /// <returns>C# instance of Type</returns>
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
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <param name="type">C# type</param>
    /// <param name="stringLiteral">Literal</param>
    /// <returns>C# instance of Type</returns>
    public virtual object From(Type type, StringLiteral stringLiteral) {
        if(type == typeof(string)) return (object) stringLiteral.Value;
        if(type.IsEnum) {
            var parsed = Enum.TryParse(type, stringLiteral.Value, true, out var @enum);
            if(parsed && @enum != null) return @enum;
            else return (object) 0;
        }

        throw new ConvertionException(LiteralType.String, type);
    }
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <param name="type">C# type</param>
    /// <param name="numericLiteral">Literal</param>
    /// <returns>C# instance of Type</returns>
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
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <param name="type">C# type</param>
    /// <param name="booleanLiteral">Literal</param>
    /// <returns>C# instance of Type</returns>
    public virtual object From(Type type, BooleanLiteral booleanLiteral) {
        if(type == typeof(string)){
            return (object) booleanLiteral.RawValue;
        }
        if(type == typeof(int)) return (object) Convert.ToInt32(booleanLiteral.Value);
        if(type == typeof(float)) return (object) Convert.ToInt32(booleanLiteral.Value);

        throw new ConvertionException(LiteralType.Boolean, type);
    }

    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <typeparam name="TType">C# type</typeparam>
    /// <param name="literal">Literal</param>
    /// <returns></returns>
    public TType From<TType>(Literal literal) => (TType) From(typeof(TType), literal);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <typeparam name="TType">C# type</typeparam>
    /// <param name="stringLiteral">Literal</param>
    /// <returns></returns>
    public TType From<TType>(StringLiteral stringLiteral) => (TType) From(typeof(TType), stringLiteral);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <typeparam name="TType">C# type</typeparam>
    /// <param name="numericLiteral">Literal</param>
    /// <returns></returns>
    public TType From<TType>(NumericLiteral numericLiteral) => (TType) From(typeof(TType), numericLiteral);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <typeparam name="TType">C# type</typeparam>
    /// <param name="booleanLiteral">Literal</param>
    /// <returns></returns>
    public TType From<TType>(BooleanLiteral booleanLiteral) => (TType) From(typeof(TType), booleanLiteral);
}