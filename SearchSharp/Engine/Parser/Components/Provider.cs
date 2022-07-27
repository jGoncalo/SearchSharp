namespace SearchSharp.Engine.Parser.Components;

public class Provider : QueryItem {
    public readonly string? ProviderId;
    public readonly string? EngineAlias;

    private Provider(string? providerId, string? engineAlias) {
        ProviderId = providerId;
        EngineAlias = engineAlias;
    }

    public static Provider With(string providerId, string engineAlias) => new Provider(providerId, engineAlias);
    public static Provider WithEngine(string engineAlias) => new Provider(null, engineAlias);
    public static Provider WithProvider(string providerId) => new Provider(providerId, null);
}