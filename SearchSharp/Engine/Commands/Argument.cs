using SearchSharp.Items;

namespace SearchSharp.Engine.Commands;

public class Argument {
    public class Runtime {
        public readonly string Identifier;
        public readonly Literal Literal;

        public Runtime(string identifier, Literal lit) {
            Identifier = identifier;
            Literal = lit;
        }
    }

    public string Identifier { get; }
    public LiteralType Type { get; }

    public Argument(string identifier, LiteralType type) {
        Identifier = identifier;
        Type = type;
    }
}