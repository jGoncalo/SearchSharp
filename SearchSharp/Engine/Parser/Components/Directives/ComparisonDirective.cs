using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Parser.Components.Directives;

public record ComparisonDirective(DirectiveComparisonOperator Operator, string Identifier, Literal Value) 
    : Directive(DirectiveType.Comparison, Identifier) {

    public override string ToString() {
        var opStr = Operator switch {
            DirectiveComparisonOperator.Rule => ":",
            DirectiveComparisonOperator.Equal => "=",
            DirectiveComparisonOperator.Similar => "~",

            _ => Operator.ToString()
        };

        return Identifier + opStr + Value.ToString();
    }
}
