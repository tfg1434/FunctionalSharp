namespace FunctionalSharp;

/// <summary>
/// Represents a cancelled error. Does not contain an exception
/// </summary>
public record class CancelledError : Error {
    public CancelledError() : base("Cancelled") { }
    
    public CancelledError(string message) : base(message) { }
}

/// <summary>
/// Represents an IO error. May or may not contain an exception
/// </summary>
public record class IOError : Error {
    public IOError() : base("IO error occurred") { }
    
    public IOError(string message) : base(message) { }

    public IOError(Exception ex) : base(ex) { }
    
    public IOError(string message, Exception ex) : base(message, ex) { }
}


