namespace SearchSharp.Engine.Parser.Components.Expressions;

public abstract class Expression : QueryItem {
    public readonly ExpType Type;

    public Expression(ExpType type) {
        Type = type;
    }

    public static Query operator +(Provider provider, Expression expression) => new Query(expression, provider);
    public static Query operator +(CommandExpression commands, Expression expression) => new Query(expression, null, commands.Commands);
}