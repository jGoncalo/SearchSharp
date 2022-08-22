using System;

namespace SearchSharp.Exceptions;

/// <summary>
/// Caused by resolution of a command argument
/// ex:
///     - received and expected types missmatch
///     - list of diferent types
/// </summary>
public class ArgumentResolutionException : Exception {
    /// <summary>
    /// Caused by resolution of a command argument
    /// </summary>
    /// <param name="message">Exception message</param>
    public ArgumentResolutionException(string message) : base(message) {

    }
    /// <summary>
    /// Caused by resolution of a command argument
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="inner">Inner exception</param>
    public ArgumentResolutionException(string message, Exception inner) 
        : base(message, inner) {

    }
}