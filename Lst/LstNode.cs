using System;
using System.Diagnostics;

namespace FPLibrary {
    public readonly partial struct Lst<T> {
        [DebuggerDisplay("Value = {Value}")]
        internal sealed class Node {
            internal Node Next { get; }
            internal readonly T Value;

            internal Node(T value) => Value = value;
            internal Node(T value, Node next) => (Value, Next) = (value, next);
        }
    }
}
