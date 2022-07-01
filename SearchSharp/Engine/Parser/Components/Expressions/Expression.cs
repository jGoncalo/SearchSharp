namespace SearchSharp.Engine.Parser.Components.Expressions;

public abstract class Expression : QueryItem {
    public readonly ExpType Type;

    public Expression(ExpType type) {
        Type = type;
    }
}