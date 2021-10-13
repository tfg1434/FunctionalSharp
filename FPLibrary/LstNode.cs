namespace FPLibrary {
    public readonly partial struct Lst<T> {
        internal sealed class Node {
            internal readonly T Value;

            internal Node(T value) => Value = value;

            internal Maybe<Node> Next { get; init; }

            public override string ToString() => $"Value: {Value} Next: {Next}";
        }
    }
}