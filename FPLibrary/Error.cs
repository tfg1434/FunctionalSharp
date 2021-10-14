namespace FPLibrary {
    public record Error(string Message) {
        public override string ToString() => $"Error({Message})";
    }
}