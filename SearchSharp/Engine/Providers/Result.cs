namespace SearchSharp.Engine.Providers;

public record Result<TQueryData>(int Count, TQueryData[] Content) where TQueryData : QueryData {
    public static Result<TQueryData> Empty => new Result<TQueryData>(0, Array.Empty<TQueryData>());
}