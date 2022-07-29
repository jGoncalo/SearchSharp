namespace SearchSharp.Engine.Parser.Components.Expressions;

public class DirectiveExpression : LogicExpression {
    public readonly Directive Directive;

    public DirectiveExpression(Directive directive) : base(ExpType.Directive) {
        Directive = directive;
    }

    public override string ToString() => Directive?.ToString() ?? string.Empty;
}