using System;

namespace SearchSharp.Exceptions;

public class ArgumentResolutionException : Exception {
    public ArgumentResolutionException(string message) : base(message) {

    }
    public ArgumentResolutionException(string message, Exception inner) 
        : base(message, inner) {

    }
}