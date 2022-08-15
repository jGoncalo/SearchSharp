using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

public record Provider(string? EngineAlias, string? ProviderId) : QueryItem {
    public static Provider None => new Provider(null, null);
    public static Provider With(string engineAlias, string providerId) => new Provider(engineAlias, providerId);
    public static Provider WithEngine(string engineAlias) => new Provider(engineAlias, null);
    public static Provider WithProvider(string providerId) => new Provider(null, providerId);

    public override string ToString() => this switch {
        { EngineAlias: null, ProviderId: null} => string.Empty,
        _ => $"<{ProviderId}@{EngineAlias}>"
    };

    public static Query operator +(Provider provider, Constraint constraint) => new Query(provider, CommandExpression.Empty, constraint);
    public static Query operator +(Provider provider, Query query) => new Query(provider, query.CommandExpression, query.Constraint);

    public static Query operator +(Provider provider, CommandExpression commandExpression) => new Query(provider, commandExpression, Constraint.None);
}