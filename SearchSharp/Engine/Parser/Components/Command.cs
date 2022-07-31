using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

public class Command : QueryItem {
    public readonly string Identifier;
    public Arguments Arguments;

    private Command(string identifer, Arguments arguments) {
        Identifier = identifer;
        Arguments = arguments;
    }
    
    public static Command NoArgument(string identifier) => new Command(identifier, new Arguments());
    public static Command WithArguments(string identifer, params Literal[] literals) => new Command(identifer, new Arguments(literals));
    public static Command WithArguments(string identifier, Arguments arguments) => new Command(identifier, arguments);

    public override string ToString()
    {
        var argStr = Arguments.ToString();
        argStr = string.IsNullOrWhiteSpace(argStr) ? string.Empty : $"({argStr})";
        return '#' + Identifier + argStr;
    }

    public static CommandExpression operator +(Command left, Command right) => new CommandExpression(left, right);
}