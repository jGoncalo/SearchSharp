using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Converters;

public interface IConverter {
    object From(Type type, Literal literal);
    object From(Type type, StringLiteral stringLiteral);
    object From(Type type, NumericLiteral numericLiteral);
    object From(Type type, BooleanLiteral booleanLiteral);

    TType From<TType>(Literal literal);
    TType From<TType>(StringLiteral stringLiteral);
    TType From<TType>(NumericLiteral numericLiteral);
    TType From<TType>(BooleanLiteral booleanLiteral);
}