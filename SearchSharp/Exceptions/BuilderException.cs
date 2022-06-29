namespace SearchSharp.Exceptions;

public class BuildException : SearchExpception {
    public BuildException(string message) : base(message) {

    }
    public BuildException(string message, Exception inner) 
        : base(message, inner) {

    }
}