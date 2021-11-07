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

public record class MaybeExError : Error {
    public MaybeExError() { }
    
    public MaybeExError(string message) : base(message) { }

    public MaybeExError(Exception ex) : base(ex.Message) => Ex = ex;
    
    public MaybeExError(string message, Exception ex) : base(message) => Ex = ex;

    public Maybe<Exception> Ex { get; init; }

    public bool HasEx => Ex.IsJust;

    public Exception ToEx()
        => Ex.Match(() => throw new InvalidOperationException("Error does not contain an exception"),
            ex => ex);

    public bool Is<E>() where E : Exception
        => Ex.Map(ex => ex is E).GetOr(false);
}

