using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

public record Query(Provider Provider, CommandExpression CommandExpression, Constraint Constraint) {
    public override string ToString() => string.Join(' ', new [] { 
            Provider.ToString(),
            string.Join(' ', CommandExpression.Commands.Select(cmd => cmd.ToString())),
            Constraint.ToString()
        }.Where(str => !string.IsNullOrWhiteSpace(str)));

    public static Query operator+(Query query, Provider provider) => new Query(provider, query.CommandExpression, query.Constraint);
    public static Query operator+(Query query, CommandExpression commandExpression) => new Query(query.Provider, query.CommandExpression + commandExpression, query.Constraint);
    public static Query operator+(Query query, Constraint constraint) => new Query(query.Provider, query.CommandExpression, constraint);
}