using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

public record Constraint(Expression Root) : QueryItem {
    public bool HasExpression => Root.Type != ExpType.None;

    public override string ToString() => Root.ToString();

    public static Constraint None => new Constraint(Expression.None);

    public static Query operator+(Constraint constraint, Query query) => new Query(query.Provider, query.CommandExpression, constraint);
    public static Query operator+(Constraint constraint, Provider provider) => new Query(provider, CommandExpression.Empty, constraint);
    public static Query operator+(Constraint constraint, CommandExpression commandExpression) => new Query(Provider.None, commandExpression, constraint);
}