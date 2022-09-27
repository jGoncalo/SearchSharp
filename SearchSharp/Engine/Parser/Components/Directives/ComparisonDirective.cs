using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Parser.Components.Directives;

/// <summary>
/// DQL Comparison Directive - Directive where data must match some pre defined comparison
/// </summary>
/// <param name="Operator">Comparison type</param>
/// <param name="Identifier">Comparison unique identifier</param>
/// <param name="Value">DQL Literal that data must be compared with</param>
public record ComparisonDirective(DirectiveComparisonOperator Operator, string Identifier, Literal Value) 
    : Directive(DirectiveType.Comparison, Identifier) {

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => Identifier + Operator.AsOp() + Value.ToString();
}
