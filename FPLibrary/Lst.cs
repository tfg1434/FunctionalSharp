using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public static partial class F {
        public static Lst<T> List<T>(IEnumerable<T> items) => Lst<T>.Create(items);

        public static Lst<T> List<T>(params T[] items) => Lst<T>.Create(items);

        public static Lst<T> ToLst<T>(this IEnumerable<T> src) => List(src);
    }

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(ImmutableEnumerableDebuggerProxy<>))]
    public readonly partial struct Lst<T> : IReadOnlyCollection<T>, IEquatable<Lst<T>> {
        private readonly Node? _head;
        private readonly int _count;

        private Lst(Node? head, int count) {
            _head = head;
            _count = count;
        }
        
        private bool isEmpty => Count == 0;
        public int Count => _count;
        public static Lst<T> Empty => default;
        
        public T Head {
            get {
                if (isEmpty) ThrowEmpty();

                return _head!.Value;
            }
        }

        public Maybe<T> HeadSafe => isEmpty ? Nothing : _head!.Value;

        public Lst<T> Tail {
            get {
                if (isEmpty) ThrowEmpty();

                return new(_head!.Next, _count - 1);
            }
        }

        public Maybe<Lst<T>> TailSafe => isEmpty ? Nothing : new Lst<T>(_head!.Next, _count - 1);
        
        public R Match<R>(Func<R> empty, Func<T, Lst<T>, R> cons)
            => isEmpty ? empty() : cons(Head, Tail);

        private static void ThrowEmpty() => throw new InvalidOperationException("Lst is empty");
        
        

        // public Maybe<T> HeadSafe => _head.Map(node => node.Value);
        //
        // public Lst<T> Tail => new(_head.NotNothing(), Count - 1);
        //
        // public Maybe<Lst<T>> TailSafe {
        //     get {
        //         int count = Count;
        //
        //         return _head.Map<Lst<T>>(h => new(h.Next, count - 1));
        //     }
        // }
        //
        // public int Count { get; }
        // private bool isEmpty => Count == 0;
        //
        // internal static Lst<T> Create(IEnumerable<T> items) {
        //     if (items is Lst<T> list) return list;
        //     if (!items.Any()) return Empty;
        //
        //     (Node Node, int Count) head = items
        //         .Reverse()
        //         .Aggregate<T, (Node Node, int Count)>(
        //             (default!, 0),
        //             (acc, t)
        //                 => (new(t) {
        //                     Next = acc.Node,
        //                 }, acc.Count + 1)
        //         );
        //
        //     return new(head.Node, head.Count);
        //
        //     //(Node? Node, int Count) head = items
        //     //    .Reverse()
        //     //    .Aggregate<T, (Node? Node, int Count)>(
        //     //        (default /* null */, 0),
        //     //        (acc, t)
        //     //            => (new Node(t) {
        //     //                Next = acc.Node == default ? Nothing : acc.Node
        //     //            }, acc.Count + 1)
        //     //    );
        //
        //     //return head.Node == default ? Empty : new Lst<T>(head.Node, head.Count);
        // }

        
    }
}