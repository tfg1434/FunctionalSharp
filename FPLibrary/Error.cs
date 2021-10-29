using System;

namespace FPLibrary {
    public record Error(int Code, string Message, Exception Exception) {
        public override string ToString() => $"Error({Message})";
    }
}