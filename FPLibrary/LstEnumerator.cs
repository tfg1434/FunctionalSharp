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
            
            //behaviour is undefined before enumeration begins and after ends
            public T Current { get; private set; }

            internal Enumerator(Node? first) {
                _next = first;
                Current = default!;
            }

            object? IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext() {
                if (_next is null) {
                    Current = default!;

                    return false;
                }
                
                Current = _next.Value;
                _next = _next.Next;

                return true;
            }

            void IEnumerator.Reset() => throw new NotSupportedException();
        }
    }
}