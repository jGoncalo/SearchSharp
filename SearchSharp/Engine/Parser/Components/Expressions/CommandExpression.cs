namespace SearchSharp.Engine.Parser.Components.Expressions;

public record CommandExpression : Expression {
    public Command[] Commands { get; init; }

    public CommandExpression(params Command[] commands) : base(ExpType.Command) {
        Commands = commands;
    }

    public static CommandExpression Empty => new CommandExpression();

    private static CommandExpression Join(CommandExpression orig, CommandExpression @new) {
        var list = orig.Commands.ToList();
        list.AddRange(@new.Commands);
        return new CommandExpression(list.ToArray());
    }

    public override string ToString() => string.Join(',', Commands.Select(cmd => cmd.ToString()));

    public static CommandExpression operator +(CommandExpression left, Command right) => Join(left, new CommandExpression(right));
    public static CommandExpression operator +(Command left, CommandExpression right) => Join(right, new CommandExpression(left));
    public static CommandExpression operator +(CommandExpression left, CommandExpression right) => Join(left, right);
    
    public static Query operator +(CommandExpression commandExpression, Constraint constraint) => new Query(Provider.None, commandExpression, constraint);
    public static Query operator +(CommandExpression commandExpression, Query query) => new Query(query.Provider, commandExpression + query.CommandExpression, query.Constraint);

    public static Query operator +(CommandExpression commandExpression, Provider provider) => new Query(provider, commandExpression, Constraint.None);
}