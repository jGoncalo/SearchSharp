using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

public class Command : QueryItem {
    public class Argument : QueryItem {
        public readonly Literal Literal;

        public Argument(Literal literal){
            Literal = literal;
        }

        public override string ToString() => Literal.RawValue;
    }

    public readonly string Identifier;
    public Argument[] Arguments;

    private Command(string identifer, params Argument[] arguments) {
        Identifier = identifer;
        Arguments = arguments ?? Array.Empty<Argument>();
    }
    
    public static Command NoArgument(string identifier) => new Command(identifier);
    public static Command WithArguments(string identifer, params Argument[] arguments) => new Command(identifer, arguments);
    public static Command WithArguments(string identifer, params Literal[] arguments) => new Command(identifer, arguments.Select(lit => new Argument(lit)).ToArray());

    public override string ToString()
    {
        var argStr = string.Join(' ', Arguments.Select(arg => arg.ToString()));
        argStr = string.IsNullOrWhiteSpace(argStr) ? string.Empty : $"({argStr})";
        return '#' + Identifier + argStr;
    }

    public static CommandExpression operator +(Command left, Command right) => new CommandExpression(left, right);
}