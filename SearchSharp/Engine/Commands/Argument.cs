using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Commands;

public record Argument(string Identifier, Literal Literal) : ArgumentDeclaration(Identifier, Literal.Type);