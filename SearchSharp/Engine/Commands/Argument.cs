using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Commands;

public class Argument : ArgumentDeclaration {
    public readonly Literal Literal;

    public Argument(string identifier, Literal literal) 
        : base(identifier, literal.Type)
    {
        Literal = literal;
    }
}