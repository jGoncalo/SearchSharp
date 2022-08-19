namespace SearchSharp.Exceptions;

public class ConvertionException : SearchExpception {
    public readonly LiteralType LiteralType;
    public readonly Type Type;

    public ConvertionException(LiteralType literalType, Type type) : base($"Could not convert from literal {literalType} to {type}") {
        LiteralType = literalType;
        Type = type;
    }
    public ConvertionException(LiteralType literalType, Type type, Exception inner) 
        : base($"Could not convert from literal {LiteralType.String} to {type}", inner) {
        LiteralType = literalType;
        Type = type;
    }
}