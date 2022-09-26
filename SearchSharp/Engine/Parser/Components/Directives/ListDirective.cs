using SearchSharp.Exceptions;

namespace SearchSharp.Engine.Parser.Components.Directives;

/// <summary>
/// DQL List directive - Directive must follow some predefined rule based on contained argument list
/// </summary>
public record ListDirective : Directive {
    /// <summary>
    /// Directive list values
    /// </summary>
    public readonly Arguments Arguments;

    /// <summary>
    /// Is directive string only
    /// </summary>
    public bool IsStringList => Arguments.IsStringList;
    /// <summary>
    /// Is directive numeric only
    /// </summary>
    public bool IsNumericList => Arguments.IsNumericList;
    /// <summary>
    /// Is directive boolean only
    /// </summary>
    public bool IsBooleanList => Arguments.IsBooleanList;

    /// <summary>
    /// Create a new List directive
    /// (arguments must be of same type)
    /// </summary>
    /// <param name="arguments">DQL Arguments - List values</param>
    /// <param name="identifier">Unique directive identifier</param>
    /// <exception cref="ArgumentResolutionException">When all arguments are not of the same type</exception>
    public ListDirective(Arguments arguments, string identifier) : base(DirectiveType.List, identifier) {
        if(!(arguments.IsStringList || arguments.IsNumericList || arguments.IsBooleanList)) {
            throw new ArgumentResolutionException("ListDirective arguments must be only of one type: [String, Numeric, Boolean]");
        }

        Arguments = arguments;
    }
    
    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => Identifier + $"[{Arguments.ToString()}]";
}