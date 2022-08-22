using System;

namespace SearchSharp.Exceptions;

/// <summary>
/// Base Searchsharp exception
/// </summary>
public class SearchExpception : Exception {
    /// <summary>
/// Base Searchsharp exception
    /// </summary>
    /// <param name="message">Exception message</param>
    public SearchExpception(string message) : base(message) {

    }
    /// <summary>
    /// Base Searchsharp exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="inner">Inner exception</param>
    public SearchExpception(string message, Exception inner) 
        : base(message, inner) {

    }
}