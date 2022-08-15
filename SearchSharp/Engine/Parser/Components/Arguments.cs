using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp.Engine.Parser.Components;

public record Arguments : QueryItem {
    public Literal[] Literals { get; init; }

    public bool IsStringList => Literals.All(lit => lit.Type == LiteralType.String);
    public bool IsNumericList => Literals.All(lit => lit.Type == LiteralType.Numeric);
    public bool IsBooleanList => Literals.All(lit => lit.Type == LiteralType.Boolean);

    public Arguments(params Literal[] literals){
        Literals = literals;
    }

    public override string ToString() => string.Join(',', Literals.Select(lit => lit.ToString()));

    public static Arguments operator +(Arguments args, Literal lit) {
        var argList = args.Literals.ToList();
        argList.Add(lit);
        return new Arguments(argList.ToArray());
    }
    public static Arguments operator +(Literal lit, Arguments args) {
        var argList = args.Literals.ToList();
        argList.Insert(0, lit);
        return new Arguments(argList.ToArray());
    }
    public static Arguments operator +(Arguments left, Arguments right) {
        var argList = left.Literals.ToList();
        argList.AddRange(right.Literals);
        return new Arguments(argList.ToArray());
    }
}