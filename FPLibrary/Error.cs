using System;
using Unit = System.ValueTuple;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public record class Error(string Message, Exception? Ex) {
        public Maybe<Exception> Exception => JustIfNotNull(Ex);

        public static Error Of(string message, Exception ex) => new(message, ex);

        public static Error Of(Exception ex) => new(ex.Message, ex);

        public static Error Of(string message) => new(message, null);
        
        public Exception ToException()
            => Ex ?? throw new InvalidOperationException("Error does not contain an exception");

        public bool IsEx<E>() where E : Exception => Ex is E;

        public override string ToString() => Message;
    }
    
    // public record Error(int Code, string Message, Exception Exception) {
    //     public override string ToString() => $"Error({Message})";
    // }
}