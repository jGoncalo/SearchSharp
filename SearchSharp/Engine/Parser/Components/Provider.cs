using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

public class Provider : QueryItem {
    public readonly string? ProviderId;
    public readonly string? EngineAlias;

    private Provider(string? engineAlias, string? providerId) {
        EngineAlias = engineAlias;
        ProviderId = providerId;
    }

    public static Provider With(string engineAlias, string providerId) => new Provider(engineAlias, providerId);
    public static Provider WithEngine(string engineAlias) => new Provider(engineAlias, null);
    public static Provider WithProvider(string providerId) => new Provider(null, providerId);

    public override string ToString() => $"<{ProviderId}@{EngineAlias}>";

    public static Query operator +(Provider provider, Query query) => new Query(query.Root, provider, query.Commands);
}