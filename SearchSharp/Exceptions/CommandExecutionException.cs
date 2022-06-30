using System;

namespace SearchSharp.Exceptions;

public class CommandExecutionException : Exception {
    public CommandExecutionException(string message) : base(message) {

    }
    public CommandExecutionException(string message, Exception inner) 
        : base(message, inner) {

    }
}