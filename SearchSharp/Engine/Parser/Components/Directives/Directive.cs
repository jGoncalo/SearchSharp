using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components.Directives;

public abstract record Directive(DirectiveType Type, string Identifier) : QueryItem {
    public DirectiveExpression AsExpression() => new DirectiveExpression(this);
}
