using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLibrary {
    public readonly partial struct Lst<T> {
        internal sealed class Node {
            internal Maybe<Node> Next { get; init; }
            internal readonly T Value;

            internal Node(T value) => Value = value;

            public override string ToString() => $"Value: {Value} Next: {Next}";
        }
    }
}
