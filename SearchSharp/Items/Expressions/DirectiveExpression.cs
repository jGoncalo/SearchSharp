namespace SearchSharp.Items.Expressions;

public class DirectiveExpression : LogicExpression {
    public readonly Directive Directive;

    public DirectiveExpression(Directive directive) : base(ExpType.Directive) {
        Directive = directive;
    }
}