using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Parser.Components.Directives;

public record NumericDirective(NumericDirective.Operator OperatorSpec, string Identifier) : Directive(DirectiveType.Numeric, Identifier) {
    public record Operator(DirectiveNumericOperator Type, NumericLiteral Value) {
        public override string ToString() => Type switch {
            DirectiveNumericOperator.Greater => ">",
            DirectiveNumericOperator.GreaterOrEqual => ">=",
            DirectiveNumericOperator.LesserOrEqual => "<=",
            DirectiveNumericOperator.Lesser => "<",

            _ => Type.ToString()
        } + Value.ToString();
    }

    public override string ToString() {
        return Identifier + OperatorSpec.ToString();
    }
}
