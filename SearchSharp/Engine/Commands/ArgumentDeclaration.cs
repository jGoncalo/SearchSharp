namespace SearchSharp.Engine.Commands;

public class ArgumentDeclaration {
    public string Identifier { get; }
    public LiteralType Type { get; }

    public ArgumentDeclaration(string identifier, LiteralType type) {
        Identifier = identifier;
        Type = type;
    }
}