using SearchSharp.Engine.Parser.Components.Expressions;

namespace SearchSharp.Engine.Parser.Components;

/// <summary>
/// DQL Query Constraint - What constraints must the data match
/// </summary>
/// <param name="Root">Root expression for a constraint</param>
public record Constraint(Expression Root) : QueryItem {
    /// <summary>
    /// If constraint has an expression
    /// </summary>
    public bool HasExpression => Root.Type != ExpType.None;

    /// <summary>
    /// To string with DQL syntax
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString() => Root.ToString();

    /// <summary>
    /// Empty Constraint
    /// </summary>
    public static Constraint None => new Constraint(Expression.None);

    /// <summary>
    /// Add a constraint to query (replace if existing)
    /// </summary>
    /// <param name="constraint">DQL Constraint</param>
    /// <param name="query">DQL Query</param>
    /// <returns>DQL Query</returns>
    public static Query operator+(Constraint constraint, Query query) => new Query(query.Provider, query.CommandExpression, constraint);
    /// <summary>
    /// Combine a constraint with a provider
    /// </summary>
    /// <param name="constraint">DQL Constraint</param>
    /// <param name="provider">DQL Provider</param>
    /// <returns>DQL Query with constraint and provider</returns>
    public static Query operator+(Constraint constraint, Provider provider) => new Query(provider, CommandExpression.Empty, constraint);
    /// <summary>
    /// Combine a constraint with a Command expression
    /// </summary>
    /// <param name="constraint">DQL Constraint</param>
    /// <param name="commandExpression">CQL Command</param>
    /// <returns>DQL Query with constraint and command expression</returns>
    public static Query operator+(Constraint constraint, CommandExpression commandExpression) => new Query(Provider.None, commandExpression, constraint);
}