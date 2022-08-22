namespace SearchSharp.Exceptions;

/// <summary>
/// Thrown when conversion to/from C# fails
/// </summary>
public class ConvertionException : SearchExpception {
    /// <summary>
    /// Literal type of failed attempted convertion
    /// </summary>
    public readonly LiteralType LiteralType;
    /// <summary>
    /// C# type of failed attempted convertion
    /// </summary>
    public readonly Type Type;

    /// <summary>
    /// Failed Literal to C# conversion
    /// </summary>
    /// <param name="literalType">Literal type</param>
    /// <param name="type">C# type</param>
    public ConvertionException(LiteralType literalType, Type type) : base($"Could not convert from literal {literalType} to {type}") {
        LiteralType = literalType;
        Type = type;
    }
    /// <summary>
    /// Failed C# to Literal conversion
    /// </summary>
    /// <param name="literalType">Literal type</param>
    /// <param name="type">C# type</param>
    public ConvertionException(Type type, LiteralType literalType) : base($"Could not convert {type} to literal {literalType}") {
        LiteralType = literalType;
        Type = type;
    }

    /// <summary>
    /// Failed Literal tp C# conversion
    /// </summary>
    /// <param name="literalType">Literal type</param>
    /// <param name="type">C# type</param>
    /// <param name="inner">Inner exception</param>
    public ConvertionException(LiteralType literalType, Type type, Exception inner) 
        : base($"Could not convert from literal {LiteralType.String} to {type}", inner) {
        LiteralType = literalType;
        Type = type;
    }
    /// <summary>
    /// Failed C# to Literal conversion
    /// </summary>
    /// <param name="literalType">Literal type</param>
    /// <param name="type">C# type</param>
    /// <param name="inner">Inner exception</param>
    public ConvertionException(Type type, LiteralType literalType, Exception inner) 
        : base($"Could not convert {type} to literal {literalType}", inner) {
        LiteralType = literalType;
        Type = type;
    }
}