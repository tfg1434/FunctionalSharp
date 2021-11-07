using System;
using Unit = System.ValueTuple;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary; 

public record class Error(string Message) {
    public Error() : this("") { }
    
    //public Error(string message)... (primary constructor)

    public Error(Exception ex) : this(ex.Message) 
        => Ex = Just(ex);
    
    public Error(string message, Exception ex) : this(message) 
        => Ex = Just(ex);
    
    public Maybe<Exception> Ex { get; init; }
    
    public bool HasEx => Ex.IsJust;
    
    public Exception ToEx()
        => Ex.Match(() => throw new InvalidOperationException("Error does not contain an exception"),
            ex => ex);
    
    public bool Is<E>() where E : Exception
        => Ex.Map(ex => ex is E).GetOr(false);
    
    public override string ToString() => $"{Message} {Ex}";
}


