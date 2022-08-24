namespace SearchSharp.Engine.Providers;

/// <summary>
/// Provider data results
/// </summary>
/// <typeparam name="TQueryData">Data type</typeparam>
/// <param name="Count">Available count</param>
/// <param name="Content">Data records</param>
public record Result<TQueryData>(int Count, TQueryData[] Content) where TQueryData : QueryData {
    /// <summary>
    /// Create empty result
    /// </summary>
    public static Result<TQueryData> Empty => new Result<TQueryData>(0, Array.Empty<TQueryData>());
}