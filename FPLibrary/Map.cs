using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FPLibrary {
    public static partial class F {
        public static Map<K, V> CreateRange<K, V>(IEnumerable<KeyValuePair<K, V>> items) where K : notnull
            => Map<K, V>.Empty.AddRange(items);
    }
    
    public sealed partial class Map<K, V> where K : notnull {
        public static readonly Map<K, V> Empty = new();
        private readonly int count;
        private readonly Node root;

        private readonly IComparer<K> keyComparer;
        private readonly IEqualityComparer<V> valComparer;

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

        public bool IsEmpty => Count == 0;
        
        #endregion

        public Map<K, V> Clear()
            => root.IsEmpty
                ? this
                : Empty.WithComparers(keyComparer, valComparer);

        public Map<K, V> SetItem(K key, V value) {
            if (key is null) throw new ArgumentNullException(nameof(key));
            
            (Node newRoot, bool replaced, _) = root.Set(keyComparer, valComparer, key, value);
            
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
                return new Map<K, V>(Node.EmptyNode, 0, keyComparer, valComparer)
                    .AddRange(this, false, false);
            }
        }

        public Map<K, V> WithDefaultComparers()
            => WithComparers(Comparer<K>.Default, EqualityComparer<V>.Default);
        
        //TODO: Add overload that takes a KeyValuePair<>
        public Map<K, V> Add(K key, V val) {
            (Node res, _) = root.Add(keyComparer, valComparer, key, val);

            return Wrap(res, count + 1);
        }
            // => root
            //     .Add(keyComparer, valComparer, key, val).Node
            //     .Pipe(
            //         Wrap
            //             .Flip()
            //             .Apply(count + 1));
        
        public Map<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> items)
            => AddRange(items, false, false);

        public bool Contains(KeyValuePair<K, V> pair) 
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
        private Map<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> items, bool overwrite, bool avoidMap) {
            //not in terms of Add so no need for new wrapper per item

            (Node Root, int Count) seed = (root, count);
            (Node Root, int Count) res = items.Aggregate(seed, (acc, item) => {
                Node node;
                bool replaced = false;
                if (overwrite)
                    (node, replaced, _) = acc.Root.Set(keyComparer, valComparer, item.Key, item.Value);
                else
                    (node, _) = acc.Root.Add(keyComparer, valComparer, item.Key, item.Value);

                return (node, replaced ? acc.Count : acc.Count + 1);
            });

            return Wrap(res.Root, res.Count);
        }

        // private Map<K, V> Wrap(Node _root, int adjustedCount)
        //     => _root == root 
        //         ? this
        //         : _root.IsEmpty
        //             ? Clear()
        //             : new(_root, adjustedCount, keyComparer, valComparer);

        private Func<Node, int, Map<K, V>> Wrap => (_root, adjustedCount) => {
            if (root != _root)
                return _root.IsEmpty ? Clear() : new(_root, adjustedCount, keyComparer, valComparer);

            return this;
        };
            // => _root == root 
            //     ? this
            //     : _root.IsEmpty
            //         ? Clear()
            //         : new(_root, adjustedCount, keyComparer, valComparer);

        internal enum KeyCollisionBehaviour {
            SetValue,
            Skip,
            ThrowIfDiff,
            ThrowAlways,
        }
    }
}

