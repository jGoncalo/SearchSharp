namespace SearchSharp.Engine.Parser.Components.Expressions;

/// <summary>
/// DQL Negated Expression - Invert expression
/// </summary>
/// <param name="Negated">Expression to be inverted</param>
public record NegatedExpression(LogicExpression Negated) : LogicExpression(ExpType.Negated) {
    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => $"!({Negated.ToString()})";
}