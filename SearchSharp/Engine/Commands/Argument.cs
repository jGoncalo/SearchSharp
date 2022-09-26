using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Commands;

/// <summary>
/// Argument for a given command execution
/// </summary>
/// <param name="Identifier">Unique argument identifier</param>
/// <param name="Literal">DQL Literal for the argument</param>
public record Argument(string Identifier, Literal Literal) : ArgumentDeclaration(Identifier, Literal.Type);