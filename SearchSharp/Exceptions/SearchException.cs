using System;

namespace SearchSharp.Exceptions;

public class SearchExpception : Exception {
    public SearchExpception(string message) : base(message) {

    }
    public SearchExpception(string message, Exception inner) 
        : base(message, inner) {

    }
}