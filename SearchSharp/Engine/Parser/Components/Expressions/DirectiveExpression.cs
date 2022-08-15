using SearchSharp.Engine.Parser.Components.Directives;

namespace SearchSharp.Engine.Parser.Components.Expressions;

public record DirectiveExpression(Directive Directive) : LogicExpression(ExpType.Directive) {
    public override string ToString() => Directive.ToString();
}