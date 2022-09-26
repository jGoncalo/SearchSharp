namespace SearchSharp.Engine.Parser.Components.Expressions;

/// <summary>
/// DQL Empty Constraint
/// </summary>
public record EmptyExpression : Expression {
    /// <summary>
    /// Empty Constraint
    /// </summary>
    public EmptyExpression() : base(ExpType.None) {}

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => string.Empty;
}