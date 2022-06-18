namespace SearchSharp.Core.Items.Expressions;

public abstract class Expression : QueryItem {
    public readonly ExpressionType Type;

    public Expression(ExpressionType type) {
        Type = type;
    }
}