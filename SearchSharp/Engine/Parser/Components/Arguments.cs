namespace SearchSharp.Engine.Parser.Components;

public class Arguments : QueryItem {
    public readonly Literal[] Literals;

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
        return args + lit;
    }
    public static Arguments operator +(Arguments left, Arguments right) {
        var argList = left.Literals.ToList();
        argList.AddRange(right.Literals);
        return new Arguments(argList.ToArray());
    }
}