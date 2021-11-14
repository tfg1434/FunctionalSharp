using System;
using System.Collections;
using System.Collections.Generic;

namespace FPLibrary {
    public readonly partial struct Lst<T> {
        public Enumerator GetEnumerator() => new(_head);

        public IEnumerable<T> AsEnumerable() {
            foreach (T item in this)
                yield return item;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T> {
            private Node? _next;

            private T? _current;

            public T Current {
                get {
                    if (_current is not null)
                        return _current;

                    throw new InvalidOperationException();
                }
            }

            internal Enumerator(Node? first) {
                _next = first;
                _current = default!;
            }

            object? IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext() {
                if (_next is null) {
                    _current = default;

                    return false;
                }
                
                _current = _next.Value;
                _next = _next.Next;

                return true;
            }

            void IEnumerator.Reset() => throw new NotSupportedException();
        }
    }
}