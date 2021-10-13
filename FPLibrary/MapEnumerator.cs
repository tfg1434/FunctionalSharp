using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FPLibrary {
    public sealed partial class Map<K, V> : IEnumerable<(K Key, V Val)> where K : notnull {
        #region IEnumerable<Tuple> Methods

        IEnumerator<(K Key, V Val)> IEnumerable<(K Key, V Val)>.GetEnumerator()
            => IsEmpty
                ? Enumerable.Empty<(K Key, V Val)>().GetEnumerator()
                : GetEnumerator();

        #endregion

        #region IEnumerable<KeyValuePair<>> Methods

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
            => AsEnumerable()
                .Map(ToKeyValuePair)
                .GetEnumerator();

        #endregion

        public IEnumerable<(K Key, V Val)> AsEnumerable() {
            foreach ((K Key, V Val) item in this)
                yield return item;
        }

        public struct Enumerator : IEnumerator<(K Key, V Val)> {
            // 1) Create an empty stack S.
            // 2) Initialize current node as root
            // 3) Push the current node to S and set current = current->left until current is NULL
            // 4) If current is NULL and stack is not empty then 
            //     a) Pop the top item from stack.
            //     b) Print the popped item, set current = popped_item->right 
            //     c) Go to step 3.
            // 5) If current is NULL and stack is empty then we are done.

            private readonly Node root;
            private readonly Stack<Node>? stack;
            private Node? current;

            internal Enumerator(Node root) {
                if (root is null) throw new ArgumentNullException(nameof(root));

                this.root = root;
                current = null;
                stack = null;

                //if passed in a non-empty tree
                if (root.IsEmpty) return;

                stack = new(root.Height);
                LeftToStack(root);
            }

            public (K Key, V Val) Current
                => current?.Value ?? throw new InvalidOperationException();

            object IEnumerator.Current => Current;

            public void Reset() {
                current = null;

                if (stack is null) return;

                stack.Clear();
                LeftToStack(root);
            }

            public void Dispose() { }

            public bool MoveNext() {
                if (stack is not null && stack.Count > 0) {
                    //stack is not empty
                    Node popped = stack.Pop();
                    current = popped;
                    LeftToStack(popped.Right!);

                    return true;
                }

                //empty stack
                current = null;

                return false;
            }

            //push provided node and it's left nodes to stack
            private void LeftToStack(Node node) {
                Debug.Assert(stack is not null);

                if (node is null) throw new ArgumentNullException(nameof(node));

                while (!node.IsEmpty) {
                    stack.Push(node);
                    node = node.Left!;
                }
            }
        }

        #region IEnumerable Methods

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public Enumerator GetEnumerator() => root.GetEnumerator();

        #endregion
    }
}