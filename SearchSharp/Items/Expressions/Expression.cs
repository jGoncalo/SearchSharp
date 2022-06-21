namespace SearchSharp.Items.Expressions;

public abstract class Expression : QueryItem {
    public readonly ExpType Type;

    public Expression(ExpType type) {
        Type = type;
    }
}