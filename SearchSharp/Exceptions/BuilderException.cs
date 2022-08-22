namespace SearchSharp.Exceptions;

/// <summary>
/// Thrown when a builder fails an operation
/// </summary>
public class BuildException : SearchExpception {
    /// <summary>
    /// Thrown when a builder fails an operation
    /// </summary>
    /// <param name="message">Reason</param>
    public BuildException(string message) : base(message) {

    }
}