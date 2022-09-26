namespace SearchSharp.Engine.Parser.Components.Expressions;

/// <summary>
/// DQL Command Expression - Expression containing multiple command invocations
/// </summary>
public record CommandExpression : Expression {
    /// <summary>
    /// Command list
    /// </summary>
    public Command[] Commands { get; init; }

    /// <summary>
    /// Create a new Command Expression
    /// </summary>
    /// <param name="commands">commands that are part of this expression</param>
    public CommandExpression(params Command[] commands) : base(ExpType.Command) {
        Commands = commands;
    }

    /// <summary>
    /// Create an empty expression
    /// </summary>
    public static CommandExpression Empty => new CommandExpression();

    private static CommandExpression Join(CommandExpression orig, CommandExpression @new) {
        var list = orig.Commands.ToList();
        list.AddRange(@new.Commands);
        return new CommandExpression(list.ToArray());
    }

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => string.Join(',', Commands.Select(cmd => cmd.ToString()));

    /// <summary>
    /// Add an command to an expression
    /// </summary>
    /// <param name="left">DQL Command Expression</param>
    /// <param name="right">DQL Command</param>
    /// <returns>DQL Command Expression containing all commands</returns>
    public static CommandExpression operator +(CommandExpression left, Command right) => Join(left, new CommandExpression(right));
    /// <summary>
    /// Add a command at the start of a command expression
    /// </summary>
    /// <param name="left">DQL Command</param>
    /// <param name="right">DQL Command Expression</param>
    /// <returns>DQL Command Expression containing all commands</returns>
    public static CommandExpression operator +(Command left, CommandExpression right) => Join(right, new CommandExpression(left));
    /// <summary>
    /// Join two command expression
    /// </summary>
    /// <param name="left">DQL Command Expression</param>
    /// <param name="right">DQL Command Expression</param>
    /// <returns>DQL Command Expression containing all commands</returns>
    public static CommandExpression operator +(CommandExpression left, CommandExpression right) => Join(left, right);
    
    /// <summary>
    /// Join a command expression and constraint into a query
    /// </summary>
    /// <param name="commandExpression">DQL Command Expression</param>
    /// <param name="constraint">DQL Constraint</param>
    /// <returns>DQL Query containing command expression and constraint</returns>
    public static Query operator +(CommandExpression commandExpression, Constraint constraint) => new Query(Provider.None, commandExpression, constraint);
    /// <summary>
    /// Join a query and command expression (if existing command expression, joins)
    /// </summary>
    /// <param name="commandExpression">DQL Command Expression</param>
    /// <param name="query">DQL Query</param>
    /// <returns>DQL Query containing join of command expressions</returns>
    public static Query operator +(CommandExpression commandExpression, Query query) => new Query(query.Provider, commandExpression + query.CommandExpression, query.Constraint);
    /// <summary>
    /// Join a command expression and a provider
    /// </summary>
    /// <param name="commandExpression">DQL Command Expression</param>
    /// <param name="provider">DQL Provider</param>
    /// <returns>DQL Query with command expression and provider</returns>
    public static Query operator +(CommandExpression commandExpression, Provider provider) => new Query(provider, commandExpression, Constraint.None);
}