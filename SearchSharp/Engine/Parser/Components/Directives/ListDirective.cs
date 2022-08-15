using SearchSharp.Exceptions;

namespace SearchSharp.Engine.Parser.Components.Directives;

public record ListDirective : Directive {
    public readonly Arguments Arguments;

    public bool IsStringList => Arguments.IsStringList;
    public bool IsNumericList => Arguments.IsNumericList;
    public bool IsBooleanList => Arguments.IsBooleanList;

    public ListDirective(Arguments arguments, string identifier) : base(DirectiveType.List, identifier) {
        var allStringList = arguments.Literals.All(lit => lit.Type == LiteralType.String);
        var allNumericList = arguments.Literals.All(lit => lit.Type == LiteralType.Numeric);
        var allBooleanList = arguments.Literals.All(lit => lit.Type == LiteralType.Boolean);

        if(!(allStringList || allNumericList || allBooleanList)) {
            throw new ArgumentResolutionException("ListDirective arguments must be only of one type: [String, Numeric, Boolean]");
        }

        Arguments = arguments;
    }

    public override string ToString()
    {
        return Identifier + $"[{Arguments.ToString()}]";
    }
}