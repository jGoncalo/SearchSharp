namespace SearchSharp.Core.Items.Expressions;

public class DirectiveExpression : LogicExpression {
    public readonly Directive Directive;

    public DirectiveExpression(Directive directive) : base(ExpressionType.Directive) {
        Directive = directive;
    }
}