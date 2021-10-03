using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FPLibrary {
    public static partial class F {
        public static Map<K, V> Map<K, V>() where K : notnull
            => FPLibrary.Map<K, V>.Empty;
        
        public static Map<K, V> Map<K, V>(IComparer<K> keyComparer) where K : notnull 
            => Map(keyComparer, EqualityComparer<V>.Default);

        public static Map<K, V> Map<K, V>(IEqualityComparer<V> valComparer) where K : notnull
            => Map(Comparer<K>.Default, valComparer);
        
        public static Map<K, V> Map<K, V>(IComparer<K> keyComparer, IEqualityComparer<V> valComparer) where K : notnull
            => FPLibrary.Map<K, V>.Empty.WithComparers(keyComparer, valComparer);

        public static Map<K, V> Map<K, V>(params (K Key, V Val)[] items) where K : notnull
            => Map(Comparer<K>.Default, EqualityComparer<V>.Default, items);

        public static Map<K, V> Map<K, V>(IComparer<K> keyComparer, params (K Key, V Val)[] items) where K : notnull
            => Map(keyComparer, EqualityComparer<V>.Default, items);

        public static Map<K, V> Map<K, V>(IEqualityComparer<V> valComparer, params (K Key, V Val)[] items) 
            where K : notnull

            => Map(Comparer<K>.Default, valComparer, items);
        
        public static Map<K, V> Map<K, V>(IComparer<K> keyComparer, IEqualityComparer<V> valComparer, 
            params (K Key, V Val)[] items) where K : notnull
        
            => FPLibrary.Map<K, V>.Empty.WithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> ToMap<T, K, V>(this IEnumerable<T> src, Func<T, K> keyProj, Func<T, V> valProj)
            where K : notnull

            => ToMap(src, Comparer<K>.Default, EqualityComparer<V>.Default, keyProj, valProj);
        
        public static Map<K, V> ToMap<T, K, V>(this IEnumerable<T> src, IComparer<K> keyComparer, Func<T, K> keyProj, 
            Func<T, V> valProj) where K : notnull
            
            => ToMap(src, keyComparer, EqualityComparer<V>.Default, keyProj, valProj);

        public static Map<K, V> ToMap<T, K, V>(this IEnumerable<T> src, IEqualityComparer<V> valComparer,
            Func<T, K> keyProj, Func<T, V> valProj) where K : notnull

            => ToMap(src, Comparer<K>.Default, valComparer, keyProj, valProj);
        
        public static Map<K, V> ToMap<T, K, V>(this IEnumerable<T> src, IComparer<K> keyComparer, 
            IEqualityComparer<V> valComparer, Func<T, K> keyProj, Func<T, V> valProj) where K : notnull {
            
            if (src is null) throw new ArgumentNullException(nameof(src));
            if (keyComparer is null) throw new ArgumentNullException(nameof(keyComparer));
            if (valComparer is null) throw new ArgumentNullException(nameof(valComparer));
            if (keyProj is null) throw new ArgumentNullException(nameof(keyProj));
            if (valProj is null) throw new ArgumentNullException(nameof(valProj));

            return FPLibrary.Map<K, V>
                .Empty
                .WithComparers(keyComparer, valComparer)
                .AddRange(src.Map(item => (keyProj(item), valProj(item))));
        }
    }
    
    public sealed partial class Map<K, V> where K : notnull {
        public static readonly Map<K, V> Empty = new();
        private readonly int count;
        private readonly Node root;

        private readonly IComparer<K> keyComparer;
        private readonly IEqualityComparer<V> valComparer;
        
        //util
        private static KeyValuePair<K, V> ToKeyValuePair((K Key, V Val) tup)
            => new(tup.Key, tup.Val);

        #region Ctors
        
        //default comparer overload
        private Map() {
            root = Node.EmptyNode;
            keyComparer = Comparer<K>.Default;
            valComparer = EqualityComparer<V>.Default;
        }
        
        internal Map(IComparer<K> keyComparer, IEqualityComparer<V> valComparer) : this() {
            this.keyComparer = keyComparer;
            this.valComparer = valComparer;
        }

        private Map(Node root, int count, IComparer<K> keyComparer, IEqualityComparer<V> valComparer) {
            root.Freeze();
            this.root = root;
            this.count = count;
            this.keyComparer = keyComparer;
            this.valComparer = valComparer;
        }

        #endregion

        #region Properties

        public int Count => count;
        public bool IsEmpty => Count == 0;
        public IComparer<K> KeyComparer => keyComparer;
        public IEqualityComparer<V> ValComparer => valComparer;
        
        #endregion

        public Map<K, V> Clear()
            => root.IsEmpty
                ? this
                : Empty.WithComparers(keyComparer, valComparer);

        public Map<K, V> SetItem((K Key, V Val) pair) {
            if (pair.Key is null) throw new ArgumentNullException($"{nameof(pair)}.{nameof(pair.Key)}");
            
            (Node newRoot, bool replaced, _) = root.Set(keyComparer, valComparer, pair);
            
            //replaced: false
            return Wrap(newRoot, replaced ? count : count + 1);
        }

        public Map<K, V> WithComparers(IComparer<K> _keyComparer,
            IEqualityComparer<V> _valComparer) {

            if (_keyComparer == keyComparer) {
                //structure doesn't depend on valComparer, so just one new node
                return _valComparer == valComparer
                    ? this
                    : new(root, count, keyComparer, _valComparer);
            } else {
                //structure does depend on keyComparer
                return new Map<K, V>(Node.EmptyNode, 0, _keyComparer, _valComparer)
                    .AddRange(this, false, true);
            }
        }

        public Map<K, V> WithDefaultComparers()
            => WithComparers(Comparer<K>.Default, EqualityComparer<V>.Default);
        
        //TODO: Add overload that takes a KeyValuePair<>
        public Map<K, V> Add((K Key, V Val) pair) {
            (Node res, _) = root.Add(keyComparer, valComparer, pair);

            return Wrap(res, count + 1);
        }

        public Map<K, V> AddRange(IEnumerable<(K Key, V Val)> items)
            => AddRange(items, false, false);

        public bool Contains((K Key, V Val) pair) 
            => root.Contains(keyComparer, valComparer, pair);

        public bool ContainsKey(K key) {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return root.ContainsKey(keyComparer, key);
        }

        public bool ContainsValue(V val) => root.ContainsValue(valComparer, val);
        
        public bool TryGetValue(K key, [MaybeNullWhen(false)] out V val) {
            if (key is null) throw new ArgumentNullException(nameof(key));
            
            return root.TryGetValue(keyComparer, key, out val);
        }

        public bool TryGetKey(K checkKey, out K realKey) {
            if (checkKey is null) throw new ArgumentNullException(nameof(checkKey));

            return root.TryGetKey(keyComparer, checkKey, out realKey);
        }
        
        //TODO: avoidMap
        private Map<K, V> AddRange(IEnumerable<(K Key, V Val)> items, bool overwrite, bool avoidMap) {
            //not in terms of Add so no need for new wrapper per item

            (Node Root, int Count) seed = (root, count);
            (Node Root, int Count) res = items.Aggregate(seed, (acc, item) => {
                Node node;
                bool replaced = false;
                if (overwrite)
                    (node, replaced, _) = acc.Root.Set(keyComparer, valComparer, item);
                else
                    (node, _) = acc.Root.Add(keyComparer, valComparer, item);

                return (node, replaced ? acc.Count : acc.Count + 1);
            });

            return Wrap(res.Root, res.Count);
        }

        private Func<Node, int, Map<K, V>> Wrap => (_root, adjustedCount) => {
            if (root != _root)
                return _root.IsEmpty ? Clear() : new(_root, adjustedCount, keyComparer, valComparer);

            return this;
        };

        internal enum KeyCollisionBehaviour {
            SetValue,
            Skip,
            ThrowIfDiff,
            ThrowAlways,
        }
    }
}

