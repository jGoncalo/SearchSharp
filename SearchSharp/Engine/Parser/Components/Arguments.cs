using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Parser.Components;

/// <summary>
/// DQL Argument list
/// </summary>
public record Arguments : QueryItem {
    /// <summary>
    /// Arguments literal list
    /// </summary>
    public Literal[] Literals { get; init; }

    /// <summary>
    /// All literals are of string literal type
    /// </summary>
    public bool IsStringList => Literals.All(lit => lit.Type == LiteralType.String);
    /// <summary>
    /// All literals are of numeric literal type
    /// </summary>
    public bool IsNumericList => Literals.All(lit => lit.Type == LiteralType.Numeric);
    /// <summary>
    /// All literals are of boolean literal type
    /// </summary>
    public bool IsBooleanList => Literals.All(lit => lit.Type == LiteralType.Boolean);
    /// <summary>
    /// Literals are of mixed types
    /// </summary>
    public bool IsMixedList => !IsStringList && !IsNumericList && !IsBooleanList;

    /// <summary>
    /// Create a new DQL Arguments from any combination of literals
    /// </summary>
    /// <param name="literals">Literals present in argument list</param>
    public Arguments(params Literal[] literals){
        Literals = literals;
    }

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => string.Join(',', Literals.Select(lit => lit.ToString()));

    /// <summary>
    /// Add literal to Arguments
    /// </summary>
    /// <param name="args">DQL Arguments</param>
    /// <param name="lit">DQL Literal</param>
    /// <returns>List containing all arguments and literal</returns>
    public static Arguments operator +(Arguments args, Literal lit) {
        var argList = args.Literals.ToList();
        argList.Add(lit);
        return new Arguments(argList.ToArray());
    }
    /// <summary>
    /// Add literal to Arguments
    /// </summary>
    /// <param name="args">DQL Arguments</param>
    /// <param name="lit">DQL Literal</param>
    /// <returns>List containing all arguments and literal</returns>
    public static Arguments operator +(Literal lit, Arguments args) {
        var argList = args.Literals.ToList();
        argList.Insert(0, lit);
        return new Arguments(argList.ToArray());
    }
    /// <summary>
    /// Combine Arguments
    /// </summary>
    /// <param name="left">DQL Arguments</param>
    /// <param name="right">DQL Arguments</param>
    /// <returns>List containing all arguments</returns>
    public static Arguments operator +(Arguments left, Arguments right) {
        var argList = left.Literals.ToList();
        argList.AddRange(right.Literals);
        return new Arguments(argList.ToArray());
    }
}

/// <summary>
/// DQL Typed Argument list
/// </summary>
/// <typeparam name="TLiteral">Type of literal for list</typeparam>
public record Arguments<TLiteral> : Arguments
    where TLiteral : Literal {

    
    /// <summary>
    /// Create a new DQL Arguments from any combination of literals
    /// </summary>
    /// <param name="literals">Literals present in argument list</param>
    public Arguments(params TLiteral[] literals) : base(literals){

    }

    /// <summary>
    /// Add literal to Arguments
    /// </summary>
    /// <param name="args">DQL Arguments</param>
    /// <param name="lit">DQL Literal</param>
    /// <returns>List containing all arguments and literal</returns>
    public static Arguments<TLiteral> operator +(Arguments<TLiteral> args, TLiteral lit) {
        var argList = args.Literals.ToList();
        argList.Add(lit);
        return new Arguments<TLiteral>(argList.Cast<TLiteral>().ToArray());
    }
    /// <summary>
    /// Add literal to Arguments
    /// </summary>
    /// <param name="args">DQL Arguments</param>
    /// <param name="lit">DQL Literal</param>
    /// <returns>List containing all arguments and literal</returns>
    public static Arguments<TLiteral> operator +(TLiteral lit, Arguments<TLiteral> args) {
        var argList = args.Literals.ToList();
        argList.Insert(0, lit);
        return new Arguments<TLiteral>(argList.Cast<TLiteral>().ToArray());
    }
    /// <summary>
    /// Combine Arguments
    /// </summary>
    /// <param name="left">DQL Arguments</param>
    /// <param name="right">DQL Arguments</param>
    /// <returns>List containing all arguments</returns>
    public static Arguments<TLiteral> operator +(Arguments<TLiteral> left, Arguments<TLiteral> right) {
        var argList = left.Literals.ToList();
        argList.AddRange(right.Literals);
        return new Arguments<TLiteral>(argList.Cast<TLiteral>().ToArray());
    }
}