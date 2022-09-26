using SearchSharp.Engine.Parser.Components.Literals;
using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

/// <summary>
/// DQL Command - What commands should affect data pre/post processing
/// </summary>
/// <param name="Identifier">Unique Command identifier</param>
/// <param name="Arguments">Command Argument list</param>
public record Command(string Identifier, Arguments Arguments) : QueryItem {
    
    /// <summary>
    /// Instance of Command with no arguments
    /// </summary>
    /// <param name="identifier">Unique Command identifier</param>
    /// <returns>DQL Command</returns>
    public static Command NoArgument(string identifier) => new Command(identifier, new Arguments());
    /// <summary>
    /// Instance of Command with arguments
    /// </summary>
    /// <param name="identifer">Unique Command identifier</param>
    /// <param name="literals">command literals</param>
    /// <returns>DQL Command</returns>
    public static Command WithArguments(string identifer, params Literal[] literals) => new Command(identifer, new Arguments(literals));
    /// <summary>
    /// Instance of Command with arguments
    /// </summary>
    /// <param name="identifier">Unique Command identifier</param>
    /// <param name="arguments">Command literals from argument</param>
    /// <returns>DQL Command</returns>
    public static Command WithArguments(string identifier, Arguments arguments) => new Command(identifier, arguments);

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString()
    {
        var argStr = Arguments.ToString();
        argStr = string.IsNullOrWhiteSpace(argStr) ? string.Empty : $"({argStr})";
        return '#' + Identifier + argStr;
    }

    /// <summary>
    /// Combine two commands into a command expression
    /// </summary>
    /// <param name="left">Command #1</param>
    /// <param name="right">Command #2</param>
    /// <returns>Command expression containing command #1 and #2</returns>
    public static CommandExpression operator +(Command left, Command right) => new CommandExpression(left, right);
}