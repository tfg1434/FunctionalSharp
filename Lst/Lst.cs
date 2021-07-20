using System;
using System.Collections.Generic;

namespace FPLibrary.Lst {
    using static F;

    //immutable singly linked list
    public readonly partial struct Lst<T> : IReadOnlyCollection<T>, IEquatable<Lst<T>> {
        public static Lst<T> Empty => default;

        private Lst(Node head, int count) => (_head, Count) = (head, count);

        public int Count { get; }

        private readonly Node _head;
        public T Head {
            get {
                if (Count == 0) ThrowEmpty();
                return _head.Value;
            }
        }

        public Lst<T> Tail {
            get {
                if (Count == 0) ThrowEmpty();
                return new Lst<T>(_head.Next, Count - 1);
            }
        }

        private static void ThrowEmpty() => throw new InvalidOperationException("The list is empty");

        private static Lst<T> create(IEnumerable<T> items) {

        }
    }
}
