using System;

namespace FPLibrary;

public record class CancelledError : Error {
    public CancelledError() : base("Cancelled") { }
    
    public CancelledError(string message) : base(message) { }
}

public record class IOError(string Message, Exception? Ex) : ExError {
    
}


