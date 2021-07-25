using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FPLibrary {
    using static F;

    public static partial class F {
        public static Lst<T> List<T>(IEnumerable<T> items) => Lst<T>.Create(items);

        public static Lst<T> List<T>(params T[] items) => Lst<T>.Create(items);

        public static Lst<T> ToLst<T>(this IEnumerable<T> src) => List(src);
    }

    //immutable singly linked list
    public readonly partial struct Lst<T> : IEquatable<Lst<T>> {
        public static Lst<T> Empty => default;

        private Lst(Node? head, int count) => (_head, Count) = (head, count);

        public int Count { get; }

        private readonly Node? _head;
        public T Head {
            get {
                if (Count == 0) ThrowEmpty();
                return _head!.Value;
            }
        }
        public Lst<T> Tail {
            get {
                if (Count == 0) ThrowEmpty();
                return new Lst<T>(_head!.Next, Count - 1);
            }
        }

        public void Deconstruct(out T head, out Lst<T> tail) {
            if (Count == 0) ThrowEmpty();
            head = _head!.Value;
            tail = new Lst<T>(_head.Next, Count - 1);
        }

        public Maybe<(T head, Lst<T> tail)> Deconstruct() {
            if (Count != 0) {
                return Just((_head!.Value, 
                    new Lst<T>(_head.Next, Count - 1)));
            }
            return Nothing;
        }

        internal static Lst<T> Create(IEnumerable<T> items) {
            //if (items is null) throw new ArgumentNullException(nameof(items));
            if (items is Lst<T> list) return list;

            int count = 0;
            Node? head = items
                .Reverse()
                .Aggregate(
                    default(Node), //null
                    (acc, t) => {
                        count++;
                        return new Node(t, acc);
                    }
                );

            return head is null ? Empty : new Lst<T>(head, count);
        }

        private static void ThrowEmpty() => throw new InvalidOperationException("The list is empty");

        //private static (Node newHead, Node newLast) CopyNonEmptyRange(Node head, Node? last) {
        //    if (head == last) throw new InvalidOperationException("range was empty");
        //}

        bool IEquatable<Lst<T>>.Equals(Lst<T> other) => _head == other._head;

        public override bool Equals(object? obj) => obj is Lst<T> list && ((IEquatable<Lst<T>>)this).Equals(list);

        public override int GetHashCode() => _head is null ? 0 : _head.GetHashCode();

        public static bool operator ==(Lst<T> self, Lst<T> other) => self.Equals(other);

        public static bool operator !=(Lst<T> self, Lst<T> other) => !(self == other);
    }
}
