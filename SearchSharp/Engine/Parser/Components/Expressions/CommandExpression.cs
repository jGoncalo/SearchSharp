namespace SearchSharp.Engine.Parser.Components.Expressions;

public class CommandExpression : Expression {
    public readonly Command[] Commands;

    public CommandExpression(params Command[] commands) : base(ExpType.Command) {
        Commands = commands;
    }

    private static CommandExpression Join(CommandExpression orig, CommandExpression @new) {
        var list = orig.Commands.ToList();
        list.AddRange(@new.Commands);
        return new CommandExpression(list.ToArray());
    }

    public override string ToString() => string.Join(',', Commands.Select(cmd => cmd.ToString()));

    public static CommandExpression operator +(CommandExpression left, Command right) => Join(left, new CommandExpression(right));
    public static CommandExpression operator +(Command left, CommandExpression right) => Join(right, new CommandExpression(left));
    public static CommandExpression operator +(CommandExpression left, CommandExpression right) => Join(left, right);
    
    public static Query operator +(CommandExpression commands, Query query) => new Query(query.Root, query.Provider, commands.Commands);
}