using System;
using SearchSharp.Engine.Parser.Components.Directives;

namespace SearchSharp.Exceptions;

/// <summary>
/// Thrown when an unknown rule is found in query
/// </summary>
public class UnknownRuleException : Exception {
    private static string ExpMessage(string identifier) => $"No rule with identifier: \"{identifier}\" found";
    
    /// <summary>
    /// Identifier of unknown command
    /// </summary>
    public readonly string Identifier;

    /// <summary>
    /// Thrown when an unknown rule is found in query
    /// </summary>
    /// <param name="identifier">Unknown command identifier</param>
    public UnknownRuleException(string identifier) : base(ExpMessage(identifier)) {
        Identifier = identifier;
    }

    /// <summary>
    /// Thrown when an unknown rule is found in query
    /// </summary>
    /// <param name="identifier">Unknown command identifier</param>
    /// <param name="message">Exception message</param>
    protected UnknownRuleException(string identifier, string message) : base(message) {
        Identifier = identifier;
    }
}

/// <summary>
/// Thrown when unknown command directive is found in query
/// </summary>
public class UnknownRuleDirectiveException : UnknownRuleException {
    private static string ExpMessage(Directive directive) => 
        $"No rule with identifier: \"{directive.Identifier}\" found that can process directive: \"{directive}\"";

    /// <summary>
    /// Unknown Directive found in query
    /// </summary>
    public readonly Directive Directive;

    /// <summary>
    /// Thrown when unknown command directive is found in query
    /// </summary>
    /// <param name="directive">Unknown Directive found in query</param>
    public UnknownRuleDirectiveException(Directive directive) : base(directive.Identifier, ExpMessage(directive)) {
        Directive = directive;
    }
}
