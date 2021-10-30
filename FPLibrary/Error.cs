using System;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public readonly record struct Error(string Message, Exception? Ex) {

        public Maybe<Exception> Exception => JustIfNotNull(Ex);

        public static Error Of(string message, Exception ex) => new(message, ex);
        
        public Exception ToException()
            => Ex ?? throw new InvalidOperationException("Error does not contain an exception");

        public bool IsEx<E>() where E : Exception => Ex is E;
    }
    
    // public record Error(int Code, string Message, Exception Exception) {
    //     public override string ToString() => $"Error({Message})";
    // }
}