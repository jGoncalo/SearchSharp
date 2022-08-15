namespace SearchSharp.Result;

public interface ISearchInput {
    public string Query { get; }
    public string EvaluatedExpression { get; }
}
