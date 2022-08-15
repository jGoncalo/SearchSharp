using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Parser.Components.Directives;

public record RangeDirective(RangeDirective.Operator OperatorSpec, string Identifier) : Directive(DirectiveType.Range, Identifier) {
    public record Operator(NumericLiteral LowerBound, NumericLiteral UpperBound) {
        public override string ToString() => $"[{LowerBound}..{UpperBound}]";
    }

    public override string ToString() => Identifier + OperatorSpec.ToString();
}
