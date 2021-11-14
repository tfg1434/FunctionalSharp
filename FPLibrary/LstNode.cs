namespace FPLibrary {
    public readonly partial struct Lst<T> {
        internal sealed class Node {
            internal readonly T Value;

            internal Node(T value) => Value = value;

            internal Node? Next { get; set; }

            public override string ToString() => $"Value: {Value} Next: {Next}";
        }
    }
}