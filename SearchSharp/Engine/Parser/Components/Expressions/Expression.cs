namespace SearchSharp.Engine.Parser.Components.Expressions;

/// <summary>
/// DQL Expression - Abstraction for all expressions
/// </summary>
/// <param name="Type">Type of expression</param>
public abstract record Expression(ExpType Type) : QueryItem {
    /// <summary>
    /// Create a constraint with this expression as root
    /// </summary>
    /// <returns>DQL Constraint</returns>
    public Constraint AsConstraint() => new Constraint(this);

    /// <summary>
    /// Create empty Constraint
    /// </summary>
    public static Expression None => new EmptyExpression();
}