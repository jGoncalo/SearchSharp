using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Engine.Commands.Runtime;

public class Argument {
    public readonly string Identifier;
    public readonly Literal Literal;

    public Argument(string identifier, Literal lit) {
        Identifier = identifier;
        Literal = lit;
    }
}