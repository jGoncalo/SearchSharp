namespace SearchSharp.Engine.Parser.Components.Expressions;

/// <summary>
/// DQL String Expression - Expression matching string
/// </summary>
/// <param name="Value">String value</param>
public record StringExpression(string Value) : LogicExpression(ExpType.String) {
    /// <summary>
    /// Empty String Expression
    /// </summary>
    public static StringExpression Empty => new StringExpression(string.Empty);

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => $"\"{Value.ToString()}\"";
}