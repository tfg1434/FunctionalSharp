using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FPLibrary.F;

namespace FPLibrary {
    using static F;

    public static partial class F {
        public static Lst<T> List<T>(IEnumerable<T> items) => Lst<T>.Create(items);

        public static Lst<T> List<T>(params T[] items) => Lst<T>.Create(items);

        public static Lst<T> ToLst<T>(this IEnumerable<T> src) => List(src);
    }

    public readonly partial struct Lst<T> {
        public static Lst<T> Empty => default;

        private readonly Maybe<Node> head;
        private readonly int count;

        private Lst(Maybe<Node> head, int count)
            => (this.head, this.count) = (head, count);

        public int Count => count;

        internal static Lst<T> Create(IEnumerable<T> items) {
            if (items is Lst<T> list) return list;

            (Node? Node, int Count) head = items
                .Reverse()
                .Aggregate<T, (Node? Node, int Count)>(
                    (default /* null */, 0),
                    (acc, t)
                        => (new Node(t) {
                            Next = acc.Node == default ? Nothing : acc.Node
                        }, acc.Count++)
                );

            return head.Node == default ? Empty : new Lst<T>(head.Node, head.Count);
        }
    }
}
