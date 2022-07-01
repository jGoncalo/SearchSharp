namespace SearchSharp.Engine.Parser.Components.Expressions;

public class CommandExpression : Expression {
    public readonly Command[] Commands;

    public CommandExpression(params Command[] commands) : base(ExpType.Command) {
        Commands = commands;
    }
    public CommandExpression(CommandExpression expr, params Command[] commands) : base(ExpType.Command) {
        var prevList = expr.Commands.ToList();
        prevList.AddRange(commands);
        Commands = prevList.ToArray();
    }
}