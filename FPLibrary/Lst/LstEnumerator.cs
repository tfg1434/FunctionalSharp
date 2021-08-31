using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FPLibrary {
    public readonly partial struct Lst<T> : IEnumerable<T> {
        public Enumerator GetEnumerator() => new(_head);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T> {
            public T Current { get; private set; }
            object? IEnumerator.Current => Current;

            private Node? next;

            internal Enumerator(Node? first) {
                next = first;
                Current = default!;
            }

            public void Dispose() { }

            public bool MoveNext() {
                if (next is null) {
                    Current = default!;
                    return false;
                }

                Current = next.Value;
                next = next.Next;
                return true;
            }

            void IEnumerator.Reset() => throw new NotSupportedException();
        }
    }
}
