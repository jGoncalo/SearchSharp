namespace SearchSharp.Result;

/// <summary>
/// Input of a given query request
/// </summary>
public class SearchInput : ISearchInput {
    /// <summary>
    /// Inputed query expression
    /// </summary>
    public string Query { get; init; } = string.Empty;
    /// <summary>
    /// Expression evaluated by the system
    /// (Expression.ToString() use for debug)
    /// </summary>
    public string EvaluatedExpression { get; init; } = string.Empty;
}
