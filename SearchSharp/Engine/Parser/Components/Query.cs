using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

/// <summary>
/// DQL Query - Root element for a query
/// </summary>
/// <param name="Provider">DQL Provider - What data provider to target for query execution</param>
/// <param name="CommandExpression">DQL Command - What commands should be applied before/after query execution</param>
/// <param name="Constraint">DQL Constraint - What constraint must be satisfied on query execution</param>
public record Query(Provider Provider, CommandExpression CommandExpression, Constraint Constraint) {
    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => string.Join(' ', new [] { 
            Provider.ToString(),
            string.Join(' ', CommandExpression.Commands.Select(cmd => cmd.ToString())),
            Constraint.ToString()
        }.Where(str => !string.IsNullOrWhiteSpace(str)));

    /// <summary>
    /// Add a provider to query (or replace)
    /// </summary>
    /// <param name="query">DQL Query</param>
    /// <param name="provider">DQL Provider</param>
    /// <returns>DQL Query with provider</returns>
    public static Query operator+(Query query, Provider provider) => new Query(provider, query.CommandExpression, query.Constraint);
    /// <summary>
    /// Add Command expression to query
    /// </summary>
    /// <param name="query">DQL Query</param>
    /// <param name="commandExpression">DQL Command Expression</param>
    /// <returns>DQL Query with merged command expression</returns>
    public static Query operator+(Query query, CommandExpression commandExpression) => new Query(query.Provider, query.CommandExpression + commandExpression, query.Constraint);
    /// <summary>
    /// Add constraint to query (or replace)
    /// </summary>
    /// <param name="query">DQL Query</param>
    /// <param name="constraint">DQL Constraint</param>
    /// <returns>DQL Query with constraint</returns>
    public static Query operator+(Query query, Constraint constraint) => new Query(query.Provider, query.CommandExpression, constraint);
}