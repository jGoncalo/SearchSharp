using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Converters;

/// <summary>
/// C#/Literal type converter
/// </summary>
public interface IConverter {
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <param name="type">C# type</param>
    /// <param name="literal">Literal</param>
    /// <returns>C# instance of Type</returns>
    object From(Type type, Literal literal);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <param name="type">C# type</param>
    /// <param name="stringLiteral">Literal</param>
    /// <returns>C# instance of Type</returns>
    object From(Type type, StringLiteral stringLiteral);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <param name="type">C# type</param>
    /// <param name="numericLiteral">Literal</param>
    /// <returns>C# instance of Type</returns>
    object From(Type type, NumericLiteral numericLiteral);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <param name="type">C# type</param>
    /// <param name="booleanLiteral">Literal</param>
    /// <returns>C# instance of Type</returns>
    object From(Type type, BooleanLiteral booleanLiteral);

    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <typeparam name="TType">C# type</typeparam>
    /// <param name="literal">Literal</param>
    /// <returns></returns>
    TType From<TType>(Literal literal);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <typeparam name="TType">C# type</typeparam>
    /// <param name="stringLiteral">Literal</param>
    /// <returns></returns>
    TType From<TType>(StringLiteral stringLiteral);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <typeparam name="TType">C# type</typeparam>
    /// <param name="numericLiteral">Literal</param>
    /// <returns></returns>
    TType From<TType>(NumericLiteral numericLiteral);
    /// <summary>
    /// Convert from literal to C# type
    /// </summary>
    /// <typeparam name="TType">C# type</typeparam>
    /// <param name="booleanLiteral">Literal</param>
    /// <returns></returns>
    TType From<TType>(BooleanLiteral booleanLiteral);
}