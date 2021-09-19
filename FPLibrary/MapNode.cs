using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FPLibrary;
using static FPLibrary.F;
using System.Linq;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : notnull {
        internal sealed class Node {
            internal static readonly Node Empty = new();
            private readonly K key = default!;
            private readonly V value = default!;
            private bool frozen;
            //AVL tree max height is 1.44 * log2(n + 2)
            private byte height;
            private Node? left;
            private Node? right;

            #region Properties

            public bool IsEmpty => left is null;

            #endregion

            #region Ctors
            private Node() => frozen = true;

            private Node(K key, V val, Node left, Node right, bool frozen=false) {
                this.key = key;
                value = val;
                this.left = left;
                this.right = right;
                height = checked((byte)(1 + Math.Max(left.height, right.height)));
                this.frozen = frozen;
            }

            #endregion

            internal void Freeze() {
                if (frozen) return;

                left!.Freeze();
                right!.Freeze();
                frozen = true;
            }

            internal (Node Node, bool Mutated) Add(IComparer<K> keyComparer,
                IEqualityComparer<V> valComparer, K _key, V val) {

                (Node node, _, bool mutated) = SetOrAdd(keyComparer, valComparer, false, _key, val);

                return (node, mutated);
            }

            internal (Node Node, bool Replaced, bool Mutated) Set(IComparer<K> keyComparer, 
                IEqualityComparer<V> valComparer, K _key, V val) {

                (Node node, bool replaced, bool mutated) = SetOrAdd(keyComparer, valComparer, true, _key, val);

                return (node, replaced, mutated);
            }

            #region Balancing methods

            /*
            a
             \
              b
               \
                c
            
            b becomes the new root
            a takes b's left node (null)
            b takes a as left
            */
            private static Node RotateLeft(Node tree) {
                if (tree is null) throw new ArgumentNullException(nameof(tree));
                Debug.Assert(!tree.IsEmpty);

                if (tree.right!.IsEmpty)
                    return tree;

                Node right = tree.right;
                return right.Mutate(
                    //right.left = tree
                    _left: tree.Mutate(
                        //tree.left = right.left
                        _right: right.left));
            }

            /*
                c
               /
              b
             /
            a

            b becomes the new root
            c takes b's right child (null) as its left child
            b takes c as it's right child
            */
            private static Node RotateRight(Node tree) {
                if (tree is null) throw new ArgumentNullException(nameof(tree));
                Debug.Assert(!tree.IsEmpty);

                if (tree.left!.IsEmpty)
                    return tree;

                Node left = tree.left;
                return left.Mutate(
                    //left.right = tree
                    _right: tree.Mutate(
                        //tree.left = left.right
                        _left: left.right));
            }
            
            /*
            a
             \
              c
             /
            b
            
            1. rotate right subtree
            a
             \
              b
               \
                c
                
            2. perform left rotation
              b
             / \
            a   c
            */
            private static Node RotateLR(Node tree) {
                if (tree is null) throw new ArgumentNullException(nameof(tree));
                
                if (tree.right!.IsEmpty)
                    return tree;

                return tree
                    .Mutate(_right: RotateRight(tree.right))
                    .Pipe(RotateLeft);
            }
            
            /*
              c
             /
            a
             \
              b
            
            1. left rotate left subtree
                c
               /
              b
             /
            a
            
            2. perform right rotation
               b
             /  \
            a    c
            */
            private static Node RotateRL(Node tree) {
                if (tree is null) throw new ArgumentNullException(nameof(tree));

                if (tree.left!.IsEmpty)
                    return tree;

                return tree
                    .Mutate(_left: RotateLeft(tree.left))
                    .Pipe(RotateRight);
            }

            private static int BalanceFactor(Node tree)
                => tree.right!.height - tree.left!.height;

            private static bool IsRightHeavy(Node tree)
                => BalanceFactor(tree) >= 2;

            private static bool IsLeftHeavy(Node tree)
                => BalanceFactor(tree) <= -2;
            
            private static Node MakeBalanced(Node tree) {
                if (tree is null) throw new ArgumentNullException(nameof(tree));
                Debug.Assert(!tree.IsEmpty);

                if (IsRightHeavy(tree))
                    return BalanceFactor(tree.right!) < 0 ? RotateLR(tree) : RotateLeft(tree);
                else if (IsLeftHeavy(tree))
                    return BalanceFactor(tree.left!) < 0 ? RotateRL(tree) : RotateRight(tree);

                return tree;
            }

            #endregion

            private Node Mutate(Node? _left=null, Node? _right=null) {
                Debug.Assert(left is not null && right is not null);

                if (frozen)
                    return new(key, value, _left ?? left, _right ?? right);

                if (_left is not null)
                    left = _left;
                if (_right is not null)
                    right = _right;
                height = checked((byte) (1 + Math.Max(left.height, right.height)));

                return this;
            }

            private (Node Node, bool Replaced, bool Mutated) SetOrAdd(IComparer<K> keyComparer,
                IEqualityComparer<V> valComparer, bool overwrite, K _key, V val) {

                //no arg validation because recursive

                if (IsEmpty)
                    return (new(_key, val, this, this), false, true);
                else {
                    //TODO: mix assignment and creation in deconstruction and/or better chaining

                    (Node Node, bool Replaced, bool Mutated) setOrAdd(Node node)
                        => node.SetOrAdd(keyComparer, valComparer, overwrite, _key, val);

                    switch (keyComparer.Compare(_key, key)) {
                        case > 0:
                            (Node newRight, bool replaced, bool mutated) = setOrAdd(right!);

                            Node res = Mutate(_right: newRight);

                            return (mutated
                                ? MakeBalanced(res)
                                : res, replaced, mutated);
                        case < 0:
                            (Node newLeft, bool lreplaced, bool lmutated) = setOrAdd(left!);

                            Node lres = Mutate(_left: newLeft);

                            return (lmutated
                                ? MakeBalanced(lres)
                                : lres, lreplaced, lmutated);
                        default:
                            if (valComparer.Equals(value, val))
                                return (this, false, false);
                            else if (overwrite)
                                return (new(_key, val, left!, right!), true, true);
                            else
                                throw new ArgumentException("Duplicate key: ", nameof(key));
                    }
                }
            }

            private Node Search(IComparer<K> keyComparer, K _key) {
                if (IsEmpty) 
                    return this;

                return keyComparer.Compare(_key, key) switch {
                    0 => this,
                    > 0 => right!.Search(keyComparer, key),
                    _ => left!.Search(keyComparer, key),
                };
            }
        }
    }
}
