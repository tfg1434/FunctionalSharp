﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FPLibrary.F;
using System.Diagnostics;
using OneOf;

namespace FPLibrary {
    using static F;

    public static partial class F {
        public static Lst<T> List<T>(IEnumerable<T> items) => Lst<T>.Create(items);

        public static Lst<T> List<T>(params T[] items) => Lst<T>.Create(items);

        public static Lst<T> ToLst<T>(this IEnumerable<T> src) => List(src);
    }

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(ImmutableEnumerableDebuggerProxy<>))]
    public readonly partial struct Lst<T> {
        public static Lst<T> Empty => default;

        private readonly Maybe<Node> head;
        public T Head {
            get => head.GetOrElse(
                () => throw new InvalidOperationException()
            ).Value;
        }
        public Maybe<T> HeadSafe {
            get => head.Map(node => node.Value);
        }

        public Lst<T> Tail {
            get => new(head.GetOrElse(
                () => throw new InvalidOperationException()
            ).Next, Count - 1);
        }
        public Maybe<Lst<T>> TailSafe {
            get {
                int count = Count;
                return head.Map<Node, Lst<T>>(head => new(head.Next, count - 1));
            }
        }

        public int Count { get; }
        private bool isEmpty => Count == 0;

        private Lst(Maybe<Node> head, int count)
            => (this.head, Count) = (head, count);

        internal static Lst<T> Create(IEnumerable<T> items) {
            if (items is Lst<T> list) return list;
            if (!items.Any()) return Empty;

            (Node Node, int Count) head = items
                .Reverse()
                .Aggregate<T, (Node Node, int Count)>(
                    (default!, 0),
                    (acc, t)
                        => (new Node(t) {
                            Next = acc.Node
                        }, acc.Count + 1)
                );

            return new Lst<T>(head.Node, head.Count);

            //(Node? Node, int Count) head = items
            //    .Reverse()
            //    .Aggregate<T, (Node? Node, int Count)>(
            //        (default /* null */, 0),
            //        (acc, t)
            //            => (new Node(t) {
            //                Next = acc.Node == default ? Nothing : acc.Node
            //            }, acc.Count + 1)
            //    );

            //return head.Node == default ? Empty : new Lst<T>(head.Node, head.Count);
        }

        //public R Match<R>(Func<R> empty, Func<T, List<T>, R> cons)
        //    => isEmpty ? empty : cons(head, tail);
    }
}