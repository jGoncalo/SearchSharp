namespace SearchSharp.Result;

/// <summary>
/// Input of a given query request
/// </summary>
public interface ISearchInput {
    /// <summary>
    /// Inputed query expression
    /// </summary>
    public string Query { get; }
    /// <summary>
    /// Expression evaluated by the system
    /// (Expression.ToString() use for debug)
    /// </summary>
    public string EvaluatedExpression { get; }
}
