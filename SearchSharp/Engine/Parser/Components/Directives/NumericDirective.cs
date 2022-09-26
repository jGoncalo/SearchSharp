using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Parser.Components.Directives;

/// <summary>
/// DQL Numeric Directive - Data must follow a pre defined rule based on numeric operator
/// </summary>
/// <param name="OperatorSpec">Numeric Directive operator specification</param>
/// <param name="Identifier">Unique directive operator</param>
public record NumericDirective(NumericDirective.Operator OperatorSpec, string Identifier) : Directive(DirectiveType.Numeric, Identifier) {
    /// <summary>
    /// DQL Numeric Directive Operator
    /// </summary>
    /// <param name="Type">Numeric Directive type</param>
    /// <param name="Value">DQL Numeric Literal to be applied on directive</param>
    public record Operator(DirectiveNumericOperator Type, NumericLiteral Value) {
        /// <summary>
        /// To string with DQL syntax
        /// </summary>
        /// <returns>String value</returns>
        public override string ToString() => Type switch {
            DirectiveNumericOperator.Greater => ">",
            DirectiveNumericOperator.GreaterOrEqual => ">=",
            DirectiveNumericOperator.LesserOrEqual => "<=",
            DirectiveNumericOperator.Lesser => "<",

            _ => Type.ToString()
        } + Value.ToString();
    }

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() {
        return Identifier + OperatorSpec.ToString();
    }
}
