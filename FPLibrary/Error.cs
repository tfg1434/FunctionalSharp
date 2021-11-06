using System;
using Unit = System.ValueTuple;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary; 

public record class Error(string Message) {
    public Error() : this("") { }
    
    //public Error(string message...)

    public override string ToString() => Message;
}

public record class ExError : Error {
    public ExError(Exception ex) : base(ex.Message) => Ex = ex;
    
    public ExError(string message, Exception ex) : base(message) => Ex = ex;

    public Exception Ex { get; init; }
    
    public override string ToString() => $"{Message} {Ex}";
}

