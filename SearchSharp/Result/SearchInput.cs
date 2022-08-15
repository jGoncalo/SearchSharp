namespace SearchSharp.Result;

public class SearchInput : ISearchInput {
    public string Query { get; init; } = string.Empty;
    public string EvaluatedExpression { get; init; } = string.Empty;
}
