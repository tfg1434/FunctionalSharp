using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FPLibrary {
    public static partial class F {
        #region Map

        public static Map<K, V> Map<K, V>() where K : notnull
            => FPLibrary.Map<K, V>.Empty;

        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
            IEqualityComparer<V>? valComparer = null) where K : notnull
            => FPLibrary.Map<K, V>.Empty.WithComparers(keyComparer, valComparer);

        public static Map<K, V> Map<K, V>(params (K Key, V Val)[] items) where K : notnull
            => Map((IEnumerable<(K Key, V Val)>) items);

        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
            IEqualityComparer<V>? valComparer = null, params (K Key, V Val)[] items) where K : notnull
            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> Map<K, V>(IEnumerable<(K Key, V Val)> items) where K : notnull
            => Map<K, V>().AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IEnumerable<(K Key, V Val)> items,
            IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull
            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        #endregion

        #region ToMap

        //to Map with no projections and default comparers
        public static Map<K, V> ToMap<K, V>(this IEnumerable<(K Key, V Val)> src) where K : notnull
            => Map(src);

        //to Map with both key and value projections
        public static Map<K, V> ToMap<T, K, V>(this IEnumerable<T> src, Func<T, K> keyProj, Func<T, V> valProj)
            where K : notnull
            => src.ToMapWithComparers(keyProj, valProj);

        //to Map with key and value comparers
        public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<(K Key, V Val)> src,
            IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull {
            if (src is null) throw new ArgumentNullException(nameof(src));

            return MapWithComparers(keyComparer, valComparer)
                .AddRange(src);
        }

        //to Map with both key and value projections & key and value comparers
        public static Map<K, V> ToMapWithComparers<T, K, V>(this IEnumerable<T> src, Func<T, K> keyProj,
            Func<T, V> valProj, IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null)
            where K : notnull
            => src
                .Map(t => (keyProj(t), valProj(t)))
                .ToMapWithComparers(keyComparer, valComparer);

        #endregion
    }

    public sealed partial class Map<K, V> where K : notnull {
        private readonly Node _root;

        #region Ctors

        //default comparer overload
        private Map() => _root = Node.EmptyNode;

        private Map(Node root, int count) {
            root.Freeze();
            _root = root;
            Count = count;
        }

        #endregion

        #region Properties

        public int Count { get; }
        public bool IsEmpty => Count == 0;
        public IComparer<K> KeyComparer { get; init; } = Comparer<K>.Default;
        public IEqualityComparer<V> ValComparer { get; init; } = EqualityComparer<V>.Default;
        public static Map<K, V> Empty => new();

        #endregion

        public bool ContainsKey(K key) {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return _root.ContainsKey(KeyComparer, key);
        }

        public bool TryGetValue(K key, [MaybeNullWhen(false)] out V val) {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return _root.TryGetValue(KeyComparer, key, out val);
        }

        public bool TryGetKey(K checkKey, out K realKey) {
            if (checkKey is null) throw new ArgumentNullException(nameof(checkKey));

            return _root.TryGetKey(KeyComparer, checkKey, out realKey);
        }

        //util
        private static KeyValuePair<K, V> ToKeyValuePair((K Key, V Val) pair)
            => new(pair.Key, pair.Val);

        private static (K Key, V Val) ToValueTuple(KeyValuePair<K, V> kv)
            => (kv.Key, kv.Value);

        private static (K Key, V Val) ToValueTuple(Tuple<K, V> tup)
            => (tup.Item1, tup.Item2);

        public Map<K, V> Clear()
            => _root.IsEmpty
                ? this
                : Empty.WithComparers(KeyComparer, ValComparer);

        public Maybe<V> Get(K key) {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return _root.Get(KeyComparer, key);
        }

        public Map<K, V> Remove(K key) {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return _root
                .Remove(KeyComparer, key)
                .Node
                .Pipe(
                    Wrap
                        .Flip()
                        .Apply(Count - 1));
        }

        public Map<K, V> RemoveRange(IEnumerable<K> keys) {
            if (keys is null) throw new ArgumentNullException(nameof(keys));

            (Node Res, int Count) seed = (_root, Count);
            (Node Res, int Count) removed = keys.Aggregate(seed, (acc, key) => {
                (var res, int count) = acc;
                (Node node, bool mutated) = res.Remove(KeyComparer, key);

                return (node, mutated ? count - 1 : count);
            });

            return Wrap(removed.Res, removed.Count);
        }

        public Map<K, V> SetItem((K Key, V Val) pair) {
            if (pair.Key is null) throw new ArgumentNullException($"{nameof(pair)}.{nameof(pair.Key)}");

            (Node newRoot, bool replaced, _) = _root.Set(KeyComparer, ValComparer, pair);
            
            return Wrap(newRoot, replaced ? Count : Count + 1);
        }

        public Map<K, V> SetItems(IEnumerable<(K Key, V Val)> items)
            => AddRange(items, true, false);

        public Map<K, V> WithComparers(IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) {
            keyComparer ??= Comparer<K>.Default;
            valComparer ??= EqualityComparer<V>.Default;

            if (keyComparer == KeyComparer) //structure doesn't depend on valComparer, so just one new node
                return valComparer == ValComparer
                    ? this
                    : new(_root, Count) { KeyComparer = KeyComparer, ValComparer = valComparer };

            return new Map<K, V>(Node.EmptyNode, 0) { KeyComparer = keyComparer, ValComparer = valComparer }
                .AddRange(this, false, true);
        }

        public Map<K, V> Add((K Key, V Val) pair) {
            (Node res, _) = _root.Add(KeyComparer, ValComparer, pair);

            return Wrap(res, Count + 1);
        }

        public Map<K, V> AddRange(IEnumerable<(K Key, V Val)> items)
            => AddRange(items, false, false);

        public bool Contains((K Key, V Val) pair)
            => _root.Contains(KeyComparer, ValComparer, pair);

        public bool ContainsVal(V val) => _root.ContainsVal(ValComparer, val);

        //TODO: avoidMap
        // ReSharper disable once UnusedParameter.Local
        private Map<K, V> AddRange(IEnumerable<(K Key, V Val)> items, bool overwrite, bool avoidMap) {
            if (items is null) throw new ArgumentNullException(nameof(items));

            //not in terms of Add so no need for new wrapper per item

            (Node Root, int Count) seed = (_root, Count);
            (Node Root, int Count) res = items.Aggregate(seed, (acc, item) => {
                Node newRes;
                bool replaced = false;
                bool mutated;
                if (overwrite)
                    (newRes, replaced, mutated) = acc.Root.Set(KeyComparer, ValComparer, item);
                else
                    (newRes, mutated) = acc.Root.Add(KeyComparer, ValComparer, item);

                return mutated ? (newRes, replaced ? acc.Count : acc.Count + 1) : (acc.Root, acc.Count);
            });

            return Wrap(res.Root, res.Count);
        }
        
        private Func<Node, int, Map<K, V>> Wrap => (root, adjustedCount) => {
            if (_root != root)
                return root.IsEmpty 
                    ? Clear() 
                    : new(root, adjustedCount) { KeyComparer = KeyComparer, ValComparer = ValComparer };

            return this;
        };
    }
}