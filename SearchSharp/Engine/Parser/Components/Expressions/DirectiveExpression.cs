using SearchSharp.Engine.Parser.Components.Directives;

namespace SearchSharp.Engine.Parser.Components.Expressions;

/// <summary>
/// DQL Expression - containting a data directive
/// </summary>
/// <param name="Directive">DQL Directive</param>
public record DirectiveExpression(Directive Directive) : LogicExpression(ExpType.Directive) {
    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => Directive.ToString();
}