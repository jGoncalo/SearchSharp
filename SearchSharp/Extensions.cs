using SearchSharp.Engine.Parser.Components.Literals;

namespace SearchSharp;

/// <summary>
/// Search sharp extensions
/// </summary>
public static class Extensions {
    #region Literal    
    /// <summary>
    /// Convert a int as a Numeric Literal
    /// </summary>
    /// <param name="value">integer value</param>
    /// <returns>Numeric Literal representation</returns>
    public static NumericLiteral AsLiteral(this int value) => NumericLiteral.Int(value);
    /// <summary>
    /// Convert a float as a Numeric Literal
    /// </summary>
    /// <param name="value">float value</param>
    /// <returns>Numeric Literal representation</returns>
    public static NumericLiteral AsLiteral(this float value) => NumericLiteral.Float(value);

    /// <summary>
    /// Convert a string as a Numeric Literal
    /// </summary>
    /// <param name="value">string value</param>
    /// <returns>Numeric Literal representation</returns>
    public static NumericLiteral AsIntLiteral(this string value) => NumericLiteral.Int(value);
    /// <summary>
    /// Convert a string as a Numeric Literal
    /// </summary>
    /// <param name="value">string value</param>
    /// <returns>Numeric Literal representation</returns>
    public static NumericLiteral AsFloatLiteral(this string value) => NumericLiteral.Float(value);
    
    /// <summary>
    /// Convert a string as a String Literal
    /// </summary>
    /// <param name="value">string value</param>
    /// <returns>String Literal representation</returns>
    public static StringLiteral AsLiteral(this string value) => new StringLiteral(value);
    /// <summary>
    /// Convert an Enum as a Numeric Literal
    /// will use Enum.ToString()
    /// </summary>
    /// <param name="value">Enum value</param>
    /// <returns>Numeric Literal representation</returns>
    public static StringLiteral AsLiteral<TEnum>(this TEnum value) where TEnum : Enum => new StringLiteral(value.ToString());

    /// <summary>
    /// Convert a bool as a Numeric Literal
    /// </summary>
    /// <param name="value">bool value</param>
    /// <returns>Numeric Literal representation</returns>
    public static BooleanLiteral AsLiteral(this bool value) => new BooleanLiteral(value);
    #endregion
}

internal static class InternalExtensions {
    #region Task
    public static TResult Await<TResult>(this Task<TResult> task) {
        (task as Task).Await();
        return task.Result;
    }
    public static void Await(this Task task) {
        Task.WhenAll(task);
        if(task.IsFaulted && task.Exception is not null) throw (task.Exception as AggregateException).GetBaseException();
    }
    #endregion
}