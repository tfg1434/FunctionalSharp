using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : notnull {
        public struct Enumerator : IEnumerator<KeyValuePair<K, V>> {
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
            
            //push provided node and it's left nodes to stack
            private void LeftToStack(Node node) {
                Debug.Assert(stack is not null);

                while (!node.IsEmpty) {
                    stack.Push(node);
                    node = node.Left!;
                }
            }
        }
    }
}