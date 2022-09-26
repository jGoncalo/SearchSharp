namespace SearchSharp.Engine.Commands;

/// <summary>
/// Argument specification for a command specification
/// </summary>
/// <param name="Identifier">Unique argument identifier</param>
/// <param name="Type">Type of argument</param>
public record ArgumentDeclaration(string Identifier, LiteralType Type);