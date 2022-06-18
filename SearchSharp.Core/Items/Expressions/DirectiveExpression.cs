namespace SearchSharp.Core.Items.Expressions;

public abstract class DirectiveExpression : ComputeExpression {
    public readonly Directive Directive;

    public DirectiveExpression(Directive directive) : base(ExpressionType.Directive) {
        Directive = directive;
    }
}