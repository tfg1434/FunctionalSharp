using System;
using System.Collections;
using System.Collections.Generic;

namespace FPLibrary {
    public readonly partial struct Lst<T> : IEnumerable<T> {
        public Enumerator GetEnumerator() => new(_head);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T> {
            private Maybe<Node> next;

            private T? current;

            public T Current {
                get {
                    if (current is not null)
                        return current;

                    throw new InvalidOperationException();
                }
            }

            internal Enumerator(Maybe<Node> first) {
                next = first;
                current = default!;
            }


            object? IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext() {
                if (next.IsNothing) {
                    current = default;

                    return false;
                }

                Node n = next.GetOr(() => default!);
                current = n.Value;
                next = n.Next;

                return true;
            }

            public void Reset() => throw new NotSupportedException();
        }
    }
}