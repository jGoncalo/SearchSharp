namespace SearchSharp.Engine.Parser.Components.Expressions;

/// <summary>
/// DQL Binary Expression - Expression must match a logic rule for two expressions
/// </summary>
/// <param name="Operator">Type of binary expression</param>
/// <param name="Left">Left logic expression</param>
/// <param name="Right">Right logic expression</param>
public record BinaryExpression(LogicOperator Operator, LogicExpression Left, LogicExpression Right) : LogicExpression(ExpType.Logic) {
    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => $"({Left.ToString()} {Operator.AsOp()} {Right.ToString()})";
}