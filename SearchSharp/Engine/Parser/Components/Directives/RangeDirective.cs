using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Parser.Components.Directives;

/// <summary>
/// DQL Range Directive - Data must follow some pre defined rule based on the range specification
/// </summary>
/// <param name="OperatorSpec">Range Operator Specification</param>
/// <param name="Identifier">Unique directive identifier</param>
public record RangeDirective(RangeDirective.Operator OperatorSpec, string Identifier) : Directive(DirectiveType.Range, Identifier) {
    /// <summary>
    /// DQL Range Directive Operator - Specifies what range should be applied on directive
    /// </summary>
    /// <param name="LowerBound">Lower Numeric value for range</param>
    /// <param name="UpperBound">Upper Numeric value for range</param>
    public record Operator(NumericLiteral LowerBound, NumericLiteral UpperBound) {
        /// <summary>
        /// To string with DQL syntax
        /// </summary>
        /// <returns>String value</returns>
        public override string ToString() => $"[{LowerBound}..{UpperBound}]";
    }

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => Identifier + OperatorSpec.ToString();
}
