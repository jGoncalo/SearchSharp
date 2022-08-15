using System;
using SearchSharp.Engine.Parser.Components.Directives;

namespace SearchSharp.Exceptions;

public class UnknownRuleException : Exception {
    protected static string ExpMessage(string identifier) => $"No rule with identifier: \"{identifier}\" found";
    
    public readonly string Identifier;

    public UnknownRuleException(string identifier) : base(ExpMessage(identifier)) {
        Identifier = identifier;
    }
    public UnknownRuleException(string identifier, Exception inner) 
        : base(ExpMessage(identifier), inner) {
        Identifier = identifier;

    }

    protected UnknownRuleException(string identifier, string message) : base(message) {
        Identifier = identifier;
    }
    protected UnknownRuleException(string identifier, string message, Exception inner) : base(message, inner) {
        Identifier = identifier;
    }
}

public class UnknownRuleDirectiveException : UnknownRuleException {
    protected static string ExpMessage(Directive directive) => 
        $"No rule with identifier: \"{directive.Identifier}\" found that can process directive: \"{directive}\"";

    public readonly Directive Directive;

    public UnknownRuleDirectiveException(Directive directive) : base(directive.Identifier, ExpMessage(directive)) {
        Directive = directive;
    }

    public UnknownRuleDirectiveException(Directive directive, Exception inner) : base(directive.Identifier, ExpMessage(directive), inner) {
        Directive = directive;
    }
}
