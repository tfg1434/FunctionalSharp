using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : notnull {
        sealed partial class Node {
            internal static readonly Node EmptyNode = new(); //so doesn't hide outer Empty
            private readonly K key = default!;
            private readonly V value = default!;

            private bool frozen;

            //AVL tree max height is 1.44 * log2(n + 2)
            private byte height;

            internal void Freeze() {
                if (frozen) return;

                Left!.Freeze();
                Right!.Freeze();
                frozen = true;
            }

            internal (Node Node, bool Mutated) Add(IComparer<K> keyComparer, IEqualityComparer<V> valComparer,
                (K Key, V Val) pair) {
                if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
                if (valComparer is null) throw new ArgumentNullException(nameof(valComparer));
                if (pair.Key is null) throw new ArgumentNullException($"{nameof(pair)}.{nameof(pair.Key)}");

                (Node node, _, bool mutated) = SetOrAdd(keyComparer, valComparer, false, pair);

                return (node, mutated);
            }

            internal (Node Node, bool Replaced, bool Mutated) Set(IComparer<K> keyComparer,
                IEqualityComparer<V> valComparer, (K Key, V Val) pair) {
                if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
                if (valComparer is null) throw new ArgumentNullException(nameof(valComparer));
                if (pair.Key is null) throw new ArgumentNullException($"{nameof(pair)}.{nameof(pair.Key)}");

                (Node node, bool replaced, bool mutated) = SetOrAdd(keyComparer, valComparer, true, pair);

                return (node, replaced, mutated);
            }

            internal bool Contains(IComparer<K> keyComparer, IEqualityComparer<V> valComparer, (K Key, V Val) pair) {
                if (pair.Key is null) throw new ArgumentException($"{nameof(pair)}.{nameof(pair.Key)}");
                if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
                if (valComparer is null) throw new ArgumentNullException(nameof(valComparer));

                Node res = Search(keyComparer, pair.Key);

                return !res.IsEmpty && valComparer.Equals(res.value, pair.Val);
            }

            internal bool ContainsKey(IComparer<K> keyComparer, K _key) {
                if (_key is null) throw new ArgumentNullException(nameof(_key));
                if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));

                return !Search(keyComparer, _key).IsEmpty;
            }

            internal bool ContainsVal(IEqualityComparer<V> valComparer, V val) {
                if (valComparer is null) throw new ArgumentNullException(nameof(valComparer));

                foreach ((_, V v) in this)
                    if (valComparer.Equals(val, v))
                        return true;

                return false;
            }

            internal Maybe<V> Get(IComparer<K> keyComparer, K _key) {
                if (_key is null) throw new ArgumentNullException(nameof(_key));

                Node res = Search(keyComparer, _key);

                return res.IsEmpty
                    ? Nothing
                    : Just(res.value);
            }

            internal (Node Node, bool Mutated) Remove(IComparer<K> keyComparer, K _key) {
                if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
                if (_key is null) throw new ArgumentNullException(nameof(_key));

                return RemoveRec(keyComparer, _key);
            }

            internal bool TryGetValue(IComparer<K> keyComparer, K _key, [MaybeNullWhen(false)] out V val) {
                if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
                if (_key is null) throw new ArgumentNullException(nameof(_key));

                Node res = Search(keyComparer, _key);

                if (res.IsEmpty) {
                    val = default;

                    return false;
                }

                val = res.value;

                return true;
            }

            internal bool TryGetKey(IComparer<K> keyComparer, K checkKey, out K realKey) {
                if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
                if (checkKey is null) throw new ArgumentNullException(nameof(checkKey));

                Node res = Search(keyComparer, checkKey);
                if (res.IsEmpty) {
                    realKey = checkKey;

                    return false;
                }

                realKey = res.key;

                return true;
            }

            internal void CopyTo(KeyValuePair<K, V>[] array, int index, int size) {
                if (array is null) throw new ArgumentNullException(nameof(array));
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length < index + size) throw new ArgumentOutOfRangeException(nameof(index));

                foreach ((K k, V v) in this)
                    array[index++] = new(k, v);
            }

            internal void CopyTo(Array array, int index, int size) {
                if (array is null) throw new ArgumentNullException(nameof(array));
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length < index + size) throw new ArgumentOutOfRangeException(nameof(index));

                foreach ((K k, V v) in this)
                    array.SetValue(new DictionaryEntry(k, v), index++);
            }

            private Node Mutate(Node? _left = null, Node? _right = null) {
                Debug.Assert(Left is not null && Right is not null);

                if (frozen)
                    return new((key, value), _left ?? Left, _right ?? Right);

                if (_left is not null)
                    Left = _left;
                if (_right is not null)
                    Right = _right;
                height = checked((byte) (1 + Math.Max(Left.height, Right.height)));

                return this;
            }

            private (Node Node, bool Replaced, bool Mutated) SetOrAdd(IComparer<K> keyComparer,
                IEqualityComparer<V> valComparer, bool overwrite, (K Key, V Val) pair) {
                //
                //TODO: mix assignment and creation in deconstruction and/or better chaining

                (Node Node, bool Replaced, bool Mutated) setOrAdd(Node node)
                    => node.SetOrAdd(keyComparer, valComparer, overwrite, pair);

                if (IsEmpty)
                    return (new(pair, this, this), false, true);

                Node res = this;
                bool replaced = false;
                bool mutated;

                switch (keyComparer.Compare(pair.Key, key)) {
                    case > 0:
                        //node goes on right 
                        Node newRight;
                        (newRight, replaced, mutated) = Right!.Pipe(setOrAdd);

                        if (mutated)
                            res = res.Mutate(_right: newRight);

                        break;
                    case < 0:
                        //node goes on left
                        Node newLeft;
                        (newLeft, replaced, mutated) = Left!.Pipe(setOrAdd);

                        if (mutated)
                            // ReSharper disable once ArgumentsStyleNamedExpression
                            res = res.Mutate(_left: newLeft);

                        break;
                    default:
                        if (valComparer.Equals(value, pair.Val)) {
                            //key and val are both the same
                            mutated = false;

                            return (this, replaced, mutated);
                        } else if (overwrite) {
                            //key exists, but val is different. mutate
                            mutated = replaced = true;
                            res = new(pair, Left!, Right!);
                        } else
                            throw new ArgumentException("Duplicate key: ",
                                $"{nameof(pair)}.{nameof(pair.Key)}");

                        break;
                }

                if (mutated)
                    res = MakeBalanced(res);

                return (res, replaced, mutated);
            }

            private Node Search(IComparer<K> keyComparer, K _key) {
                if (IsEmpty)
                    return this;

                return keyComparer.Compare(_key, key) switch {
                    0 => this,
                    > 0 => Right!.Search(keyComparer, _key),
                    < 0 => Left!.Search(keyComparer, _key),
                };
            }

            private (Node Node, bool Mutated) RemoveRec(IComparer<K> keyComparer, K _key) {
                //no validation, recursive
                //http://www.mathcs.emory.edu/~cheung/Courses/253/Syllabus/Trees/AVL-delete.html

                if (IsEmpty)
                    return (this, false);

                Debug.Assert(Right is not null && Left is not null);
                Node res = this;
                bool mutated;

                switch (keyComparer.Compare(_key, key)) {
                    //getting block scoping
                    case 0: {
                        mutated = true;

                        if (Right.IsEmpty && Left.IsEmpty)
                            //leaf
                            res = EmptyNode;
                        else if (Right.IsEmpty && !Left.IsEmpty)
                            //1 child NODE on left: connect its parent and its child
                            res = Left;
                        else if (!Right.IsEmpty && Left.IsEmpty)
                            //1 child NODE on right: connect its parent and its child
                            res = Right;
                        else {
                            //2 child nodes
                            Node successor = Right;
                            //get the inorder successor: leftmost child of right subtree
                            while (!successor.Left!.IsEmpty)
                                successor = successor.Left;

                            //remove successor and replace this node with it
                            (Node newRight, _) = Right.Remove(keyComparer, successor.key);
                            res = successor.Mutate(Left, newRight);
                        }

                        break;
                    }

                    case < 0:
                        Node newLeft;
                        (newLeft, mutated) = Left.Remove(keyComparer, _key);

                        if (mutated)
                            res = Mutate(newLeft);

                        break;
                    case > 0: {
                        Node newRight;
                        (newRight, mutated) = Right.Remove(keyComparer, _key);

                        if (mutated)
                            res = Mutate(_right: newRight);

                        break;
                    }
                }

                return (res.IsEmpty ? res : MakeBalanced(res), mutated);
            }

            #region Properties

            public bool IsEmpty => Left is null;
            public int Height => height;
            public Node? Left { get; private set; }

            public Node? Right { get; private set; }

            public (K Key, V Val) Value => (key, value);
            internal IEnumerable<K> Keys => this.Select(x => x.Key);
            internal IEnumerable<V> Values => this.Select(x => x.Val);

            #endregion

            #region Ctors

            private Node() => frozen = true;

            private Node((K Key, V Val) pair, Node left, Node right, bool frozen = false) {
                (key, value) = pair;
                Left = left;
                Right = right;
                height = checked((byte) (1 + Math.Max(left.height, right.height)));
                this.frozen = frozen;
            }

            #endregion

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

                if (tree.Right!.IsEmpty)
                    return tree;

                Node right = tree.Right;

                return right.Mutate(
                    //right.left = tree
                    tree.Mutate(
                        //tree.left = right.left
                        _right: right.Left));
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

                if (tree.Left!.IsEmpty)
                    return tree;

                Node left = tree.Left;

                return left.Mutate(
                    //left.right = tree
                    _right: tree.Mutate(
                        //tree.left = left.right
                        left.Right));
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

                if (tree.Right!.IsEmpty)
                    return tree;

                return tree
                    .Mutate(_right: RotateRight(tree.Right))
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

                if (tree.Left!.IsEmpty)
                    return tree;

                return tree
                    .Mutate(RotateLeft(tree.Left))
                    .Pipe(RotateRight);
            }

            private static int BalanceFactor(Node tree)
                => tree.Right!.height - tree.Left!.height;

            private static bool IsRightHeavy(Node tree)
                => BalanceFactor(tree) >= 2;

            private static bool IsLeftHeavy(Node tree)
                => BalanceFactor(tree) <= -2;

            private static Node MakeBalanced(Node tree) {
                if (tree is null) throw new ArgumentNullException(nameof(tree));
                Debug.Assert(!tree.IsEmpty);

                if (IsRightHeavy(tree))
                    return BalanceFactor(tree.Right!) < 0 ? RotateLR(tree) : RotateLeft(tree);
                if (IsLeftHeavy(tree))
                    return BalanceFactor(tree.Left!) < 0 ? RotateRL(tree) : RotateRight(tree);

                return tree;
            }

            #endregion
        }
    }
}