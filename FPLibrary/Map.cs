using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FPLibrary {
    public static partial class F {
        #region Map
        
        public static Map<K, V> Map<K, V>() where K : notnull
            => FPLibrary.Map<K, V>.Empty;
        
        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer=null, 
            IEqualityComparer<V>? valComparer=null) where K : notnull

            => FPLibrary.Map<K, V>.Empty.WithComparers(keyComparer, valComparer);

        public static Map<K, V> Map<K, V>(params (K Key, V Val)[] items) where K : notnull
            => Map((IEnumerable<(K Key, V Val)>) items);

        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer=null, 
            IEqualityComparer<V>? valComparer=null, params (K Key, V Val)[] items) where K : notnull

            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> Map<K, V>(IEnumerable<(K Key, V Val)> items) where K : notnull
            => Map<K, V>().AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IEnumerable<(K Key, V Val)> items, 
            IComparer<K>? keyComparer=null, IEqualityComparer<V>? valComparer=null) where K : notnull

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
            IComparer<K>? keyComparer=null, IEqualityComparer<V>? valComparer=null) where K : notnull {
            
            if (src is null) throw new ArgumentNullException(nameof(src));

            return MapWithComparers(keyComparer, valComparer)
                .AddRange(src);
        }
        
        //to Map with both key and value projections & key and value comparers
        public static Map<K, V> ToMapWithComparers<T, K, V>(this IEnumerable<T> src, Func<T, K> keyProj, 
            Func<T, V> valProj, IComparer<K>? keyComparer=null, IEqualityComparer<V>? valComparer=null) 
            where K : notnull {

            return src
                .Map(t => (keyProj(t), valProj(t)))
                .ToMapWithComparers(keyComparer, valComparer);
        }
        
        #endregion
    }
    
    public sealed partial class Map<K, V> where K : notnull {
        public static readonly Map<K, V> Empty = new();
        private readonly int count;
        private readonly Node root;

        private readonly IComparer<K> keyComparer;
        private readonly IEqualityComparer<V> valComparer;
        
        //util
        private static KeyValuePair<K, V> ToKeyValuePair((K Key, V Val) pair)
            => new(pair.Key, pair.Val);

        private static (K Key, V Val) ToValueTuple(KeyValuePair<K, V> kv)
            => (kv.Key, kv.Value);
        
        private static (K Key, V Val) ToValueTuple(Tuple<K, V> tup)
            => (tup.Item1, tup.Item2);

        #region Ctors
        
        //default comparer overload
        private Map() {
            root = Node.EmptyNode;
            keyComparer = Comparer<K>.Default;
            valComparer = EqualityComparer<V>.Default;
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

        public Maybe<V> Get(K key) {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return root.Get(keyComparer, key);
        }

        public Map<K, V> Remove(K key) {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return root
                .Remove(keyComparer, key)
                .Node
                .Pipe(
                    Wrap
                        .Flip()
                        .Apply(count - 1));
        }

        public Map<K, V> SetItem((K Key, V Val) pair) {
            if (pair.Key is null) throw new ArgumentNullException($"{nameof(pair)}.{nameof(pair.Key)}");
            
            (Node newRoot, bool replaced, _) = root.Set(keyComparer, valComparer, pair);
            
            //replaced: false
            return Wrap(newRoot, replaced ? count : count + 1);
        }

        public Map<K, V> SetItems(IEnumerable<(K Key, V Val)> items)
            => AddRange(items, true, false);

        public Map<K, V> WithComparers(IComparer<K>? _keyComparer=null, IEqualityComparer<V>? _valComparer=null) {
            _keyComparer ??= Comparer<K>.Default;
            _valComparer ??= EqualityComparer<V>.Default;

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

        public bool ContainsVal(V val) => root.ContainsVal(valComparer, val);
        
        public bool TryGetValue(K key, [MaybeNullWhen(false)] out V val) {
            if (key is null) throw new ArgumentNullException(nameof(key));
            
            return root.TryGetValue(keyComparer, key, out val);
        }

        public bool TryGetKey(K checkKey, out K realKey) {
            if (checkKey is null) throw new ArgumentNullException(nameof(checkKey));

            return root.TryGetKey(keyComparer, checkKey, out realKey);
        }
        
        //TODO: avoidMap
        // ReSharper disable once UnusedParameter.Local
        private Map<K, V> AddRange(IEnumerable<(K Key, V Val)> items, bool overwrite, bool avoidMap) {
            if (items is null) throw new ArgumentNullException(nameof(items));
            
            //not in terms of Add so no need for new wrapper per item

            (Node Root, int Count) seed = (root, count);
            (Node Root, int Count) res = items.Aggregate(seed, (acc, item) => {
                Node newRes;
                bool replaced = false;
                bool mutated;
                if (overwrite)
                    (newRes, replaced, mutated) = acc.Root.Set(keyComparer, valComparer, item);
                else
                    (newRes, mutated) = acc.Root.Add(keyComparer, valComparer, item);
                
                return mutated ? (newRes, replaced ? acc.Count : acc.Count + 1) : (acc.Root, acc.Count);
            });

            return Wrap(res.Root, res.Count);
        }

        private Func<Node, int, Map<K, V>> Wrap => (_root, adjustedCount) => {
            if (root != _root)
                return _root.IsEmpty ? Clear() : new(_root, adjustedCount, keyComparer, valComparer);

            return this;
        };
    }
}

