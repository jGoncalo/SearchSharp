using System;

namespace SearchSharp.Exceptions;

/// <summary>
/// Thrown when a command execution fails to be applyed
/// </summary>
public class CommandExecutionException : Exception {
    /// <summary>
    /// Thrown when a command execution fails to be applyed
    /// </summary>
    /// <param name="message">Exception message</param>
    public CommandExecutionException(string message) : base(message) {

    }
    /// <summary>
    /// Thrown when a command execution fails to be applyed
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="inner">Exception found durring command execution</param>
    public CommandExecutionException(string message, Exception inner) 
        : base(message, inner) {

    }
}