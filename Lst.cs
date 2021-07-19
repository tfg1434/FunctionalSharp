using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace FPLibrary {
    using static F;

    //immutable singly linked list

    public class Lst<T> : IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, IEquatable<Lst<T>> {

        [DebuggerDisplay("Value = {Value}")]

        //private Lst(Node head, int count)
        //    =>

        public int Count { get; }
        public T Head {
            get {
                if (Count == 0) ThrowEmpty();
                return 
            }
        }

        

        internal sealed class Node {
            internal Node Next { get; }
            internal readonly T Value;

            internal Node(T value) => Value = value;
        }

        private static void ThrowEmpty() => throw new InvalidOperationException("The list is empty");
    }
}
