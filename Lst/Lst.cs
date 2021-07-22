using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLibrary {
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

        internal static Lst<T> Create(params T[] items) {
            if (items is null) throw new ArgumentNullException(nameof(items));
            return createRange(items);
        }

        internal static Lst<T> CreateRange(IEnumerable<T> items) {
            if (items is null) throw new ArgumentNullException(nameof(items));
            return createRange(items);
        }

        private static void ThrowEmpty() => throw new InvalidOperationException("The list is empty");

        private static Lst<T> createRange(IEnumerable<T> items) {
            if (items is Lst<T> list) { return list; }
            if (!items.Any()) return Empty;

            int count = 0;
            return new Lst<T>(
                items
                    .Reverse()
                    .Aggregate(
                        default(Node), 
                        (acc, t) => {
                            count++;
                            return new(t, acc);
                        }
                    ), count);
        }
    }
}
