namespace SearchSharp.Engine.Commands;

public class Argument {
    public string Identifier { get; }
    public LiteralType Type { get; }

    public Argument(string identifier, LiteralType type) {
        Identifier = identifier;
        Type = type;
    }
}