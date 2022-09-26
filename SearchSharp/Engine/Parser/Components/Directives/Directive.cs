using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components.Directives;

/// <summary>
/// DQL Directive - abstract directive container
/// </summary>
/// <param name="Type">Type of directive</param>
/// <param name="Identifier">Unique directive identifier</param>
public abstract record Directive(DirectiveType Type, string Identifier) : QueryItem {
    /// <summary>
    /// Create an expression containing this directive as its single directive
    /// </summary>
    /// <returns>DQL Directive Expression</returns>
    public DirectiveExpression AsExpression() => new DirectiveExpression(this);
}
