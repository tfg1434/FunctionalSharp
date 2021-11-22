using System.Collections;
using System.Collections.Generic;

namespace FunctionalSharp; 

public readonly partial struct Lst<T> {
    /// <summary>
    /// Get the enumerator for the list. Value type semantics!
    /// </summary>
    /// <returns><see cref="IEnumerator{T}"/></returns>
    public Enumerator GetEnumerator() => new(_head);

    /// <summary>
    /// Return the list as an iterator
    /// </summary>
    /// <returns>Iterator for this list</returns>
    public IEnumerable<T> AsEnumerable() {
        foreach (T item in this)
            yield return item;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Enumerator for the list. Value type semantics!
    /// </summary>
    public struct Enumerator : IEnumerator<T> {
        private Node? _next;
            
        /// <summary>
        /// Current value. Behaviour is undefined before and after enumeration
        /// </summary>
        public T Current { get; private set; }

        internal Enumerator(Node? first) {
            _next = first;
            Current = default!;
        }

        object? IEnumerator.Current => Current;

        /// <summary>
        /// Dispose this enumerator. Does nothing in this case
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Advance the enumerator to the next item
        /// </summary>
        /// <returns>Whether there are items left to be enumerated</returns>
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