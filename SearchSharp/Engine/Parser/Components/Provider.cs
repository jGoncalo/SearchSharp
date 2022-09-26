using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

/// <summary>
/// DQL Provider - What data source should query be run against
/// </summary>
/// <param name="EngineAlias">Alias of the search engine</param>
/// <param name="ProviderId">Unique provider identifier</param>
public record Provider(string? EngineAlias, string? ProviderId) : QueryItem {
    /// <summary>
    /// Instance when no provider is targeted
    /// </summary>
    public static Provider None => new Provider(null, null);
    /// <summary>
    /// Instance where an exact provider is targeted
    /// </summary>
    /// <param name="engineAlias">Alias of the engine</param>
    /// <param name="providerId">Provider unique Identifier</param>
    /// <returns>DQL Provider</returns>
    public static Provider With(string engineAlias, string providerId) => new Provider(engineAlias, providerId);
    /// <summary>
    /// Instance where an exact provider is targeted
    /// </summary>
    /// <param name="engineAlias">Alias of the engine</param>
    /// <returns>DQL Provider</returns>
    public static Provider WithEngine(string engineAlias) => new Provider(engineAlias, null);
    /// <summary>
    /// Instance where an exact provider is targeted
    /// </summary>
    /// <param name="providerId">Provider unique Identifier</param>
    /// <returns>DQL Provider</returns>
    public static Provider WithProvider(string providerId) => new Provider(null, providerId);

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => this switch {
        { EngineAlias: null, ProviderId: null} => string.Empty,
        _ => $"<{ProviderId}@{EngineAlias}>"
    };

    /// <summary>
    /// Combine a provider and a constraint into a query
    /// </summary>
    /// <param name="provider">DQL Provider</param>
    /// <param name="constraint">DQL Constraint</param>
    /// <returns>DQL Query with constraint and provider</returns>
    public static Query operator +(Provider provider, Constraint constraint) => new Query(provider, CommandExpression.Empty, constraint);
    /// <summary>
    /// Add a provider to a query (replace if existing)
    /// </summary>
    /// <param name="provider">DQL Provider</param>
    /// <param name="query">DQL Query</param>
    /// <returns>DQL Query with provider</returns>
    public static Query operator +(Provider provider, Query query) => new Query(provider, query.CommandExpression, query.Constraint);
    /// <summary>
    /// Combine a provider and a command expression
    /// </summary>
    /// <param name="provider">DQL Provider</param>
    /// <param name="commandExpression">DQL Command</param>
    /// <returns>DQL Query with provider and command expression</returns>
    public static Query operator +(Provider provider, CommandExpression commandExpression) => new Query(provider, commandExpression, Constraint.None);
}