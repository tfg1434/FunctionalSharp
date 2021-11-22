using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FunctionalSharp; 

public sealed partial class Map<K, V> where K : notnull {
    sealed partial class Node {
        internal static readonly Node EmptyNode = new(); //so doesn't hide outer Empty
        private readonly K _key = default!;
        private readonly V _value = default!;

        private bool _frozen;

        //AVL tree max height is 1.44 * log2(n + 2)
        private byte _height;

        internal void Freeze() {
            if (_frozen) return;

            Left!.Freeze();
            Right!.Freeze();
            _frozen = true;
        }

        internal (Node Node, bool Mutated) Add(IComparer<K> keyComparer, IEqualityComparer<V> valueComparer, K key, 
            V value) {

            (Node node, _, bool mutated) = SetOrAdd(keyComparer, valueComparer, false, key, value);

            return (node, mutated);
        }

        internal (Node Node, bool Replaced, bool Mutated) Set(IComparer<K> keyComparer,
            IEqualityComparer<V> valueComparer, K key, V value) {
            
            (Node node, bool replaced, bool mutated) = SetOrAdd(keyComparer, valueComparer, true, key, value);

            return (node, replaced, mutated);
        }

        internal bool Contains(IComparer<K> keyComparer, IEqualityComparer<V> valueComparer, K key, V value) {
            Node res = Search(keyComparer, key);

            return !res.IsEmpty && valueComparer.Equals(res._value, value);
        }

        internal bool ContainsKey(IComparer<K> keyComparer, K key) {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));

            return !Search(keyComparer, key).IsEmpty;
        }

        internal bool ContainsValue(IEqualityComparer<V> valueComparer, V value) {
            if (valueComparer is null) throw new ArgumentNullException(nameof(valueComparer));

            foreach ((_, V curr) in this)
                if (valueComparer.Equals(value, curr))
                    return true;

            return false;
        }

        internal Maybe<V> Get(IComparer<K> keyComparer, K key) {
            if (key is null) throw new ArgumentNullException(nameof(key));

            Node res = Search(keyComparer, key);

            return res.IsEmpty
                ? Nothing
                : Just(res._value);
        }

        internal (Node Node, bool Mutated) Remove(IComparer<K> keyComparer, K key) {
            if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
            if (key is null) throw new ArgumentNullException(nameof(key));

            return RemoveRec(keyComparer, key);
        }

        internal bool TryGetValue(IComparer<K> keyComparer, K key, [MaybeNullWhen(false)] out V val) {
            if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
            if (key is null) throw new ArgumentNullException(nameof(key));

            Node res = Search(keyComparer, key);

            if (res.IsEmpty) {
                val = default;

                return false;
            }

            val = res._value;

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

            realKey = res._key;

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

        private Node Mutate(Node? left = null, Node? right = null) {
            Debug.Assert(Left is not null && Right is not null);

            if (_frozen)
                return new((_key, _value), left ?? Left, right ?? Right);

            if (left is not null)
                Left = left;
            if (right is not null)
                Right = right;
            _height = checked((byte) (1 + Math.Max(Left._height, Right._height)));

            return this;
        }

        private (Node Node, bool Replaced, bool Mutated) SetOrAdd(IComparer<K> keyComparer,
            IEqualityComparer<V> valueComparer, bool overwrite, K key, V value) {

            (Node Node, bool Replaced, bool Mutated) SetOrAddNode(Node node)
                => node.SetOrAdd(keyComparer, valueComparer, overwrite, key, value);

            if (IsEmpty)
                return (new(key, value, this, this), false, true);

            Node res = this;
            bool replaced = false;
            bool mutated;

            switch (keyComparer.Compare(key, _key)) {
                case > 0:
                    //node goes on right 
                    (Node newRight, replaced, mutated) = Right!.Pipe(SetOrAddNode);

                    if (mutated)
                        res = res.Mutate(right: newRight);

                    break;
                case < 0:
                    //node goes on left
                    (Node newLeft, replaced, mutated) = Left!.Pipe(SetOrAddNode);

                    if (mutated)
                        // ReSharper disable once ArgumentsStyleNamedExpression
                        res = res.Mutate(left: newLeft);

                    break;
                default:
                    if (valueComparer.Equals(_value, value)) {
                        //key and val are both the same
                        mutated = false;

                        return (this, replaced, mutated);
                        
                    } else if (overwrite) {
                        //key exists, but val is different. mutate
                        mutated = replaced = true;
                        res = new(key, value, Left!, Right!);
                        
                    } else
                        throw new ArgumentException("Duplicate key: ", nameof(key));

                    break;
            }

            if (mutated)
                res = MakeBalanced(res);

            return (res, replaced, mutated);
        }

        private Node Search(IComparer<K> keyComparer, K key) {
            if (IsEmpty)
                return this;

            return keyComparer.Compare(key, this._key) switch {
                0 => this,
                > 0 => Right!.Search(keyComparer, key),
                < 0 => Left!.Search(keyComparer, key),
            };
        }

        private (Node Node, bool Mutated) RemoveRec(IComparer<K> keyComparer, K key) {
            //no validation, recursive
            //http://www.mathcs.emory.edu/~cheung/Courses/253/Syllabus/Trees/AVL-delete.html

            if (IsEmpty)
                return (this, false);

            Debug.Assert(Right is not null && Left is not null);
            Node res = this;
            bool mutated;

            switch (keyComparer.Compare(key, _key)) {
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
                        (Node newRight, _) = Right.Remove(keyComparer, successor._key);
                        res = successor.Mutate(Left, newRight);
                    }

                    break;
                }

                case < 0:
                    Node newLeft;
                    (newLeft, mutated) = Left.Remove(keyComparer, key);

                    if (mutated)
                        res = Mutate(newLeft);

                    break;
                case > 0: {
                    Node newRight;
                    (newRight, mutated) = Right.Remove(keyComparer, key);

                    if (mutated)
                        res = Mutate(right: newRight);

                    break;
                }
            }

            return (res.IsEmpty ? res : MakeBalanced(res), mutated);
        }

        #region Properties

        public bool IsEmpty => Left is null;
        public int Height => _height;
        public Node? Left { get; private set; }

        public Node? Right { get; private set; }

        public (K Key, V Value) Value => (_key, _value);
        internal IEnumerable<K> Keys => this.Select(x => x.Key);
        internal IEnumerable<V> Values => this.Select(x => x.Value);

        #endregion

        #region Ctors

        private Node() => _frozen = true;

        private Node(K key, V value, Node left, Node right, bool frozen = false) {
            _key = key;
            _value = value;
            Left = left;
            Right = right;
            _height = checked((byte) (1 + Math.Max(left._height, right._height)));
            _frozen = frozen;
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
                    right: right.Left));
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
                right: tree.Mutate(
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
        private static Node RotateLr(Node tree) {
            if (tree is null) throw new ArgumentNullException(nameof(tree));

            if (tree.Right!.IsEmpty)
                return tree;

            return tree
                .Mutate(right: RotateRight(tree.Right))
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
        private static Node RotateRl(Node tree) {
            if (tree is null) throw new ArgumentNullException(nameof(tree));

            if (tree.Left!.IsEmpty)
                return tree;

            return tree
                .Mutate(RotateLeft(tree.Left))
                .Pipe(RotateRight);
        }

        private static int BalanceFactor(Node tree)
            => tree.Right!._height - tree.Left!._height;

        private static bool IsRightHeavy(Node tree)
            => BalanceFactor(tree) >= 2;

        private static bool IsLeftHeavy(Node tree)
            => BalanceFactor(tree) <= -2;

        private static Node MakeBalanced(Node tree) {
            if (tree is null) throw new ArgumentNullException(nameof(tree));
            Debug.Assert(!tree.IsEmpty);

            if (IsRightHeavy(tree))
                return BalanceFactor(tree.Right!) < 0 ? RotateLr(tree) : RotateLeft(tree);
            if (IsLeftHeavy(tree))
                return BalanceFactor(tree.Left!) < 0 ? RotateRl(tree) : RotateRight(tree);

            return tree;
        }

        #endregion
    }
}