using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : IComparable<K> {
        public static readonly Map<K, V> Empty = new();
        private readonly int count;
        private readonly Node root;

        private readonly IComparer<K> keyComparer;
        private readonly IEqualityComparer<V> valComparer;

        #region Ctors

        private Map() {
            root = Node.Empty;
            keyComparer = Comparer<K>.Default;
            valComparer = EqualityComparer<V>.Default;
        }

        internal Map(IComparer<K> keyComparer, IEqualityComparer<V> valComparer) {
            root = Node.Empty;
            this.keyComparer = keyComparer;
            this.valComparer = valComparer;
        }

        private Map(Node root, int count, IComparer<K> keyComparer,
            IEqualityComparer<V> valComparer) {

            root.Freeze();
            this.root = root;
            this.count = count;
            this.keyComparer = keyComparer;
            this.valComparer = valComparer;
        }

        #endregion

        #region Properties

        #endregion

        public Map<K, V> Clear()
            => root.IsEmpty
                ? this
                : Empty.WithComparers(keyComparer, valComparer);

        public Map<K, V> SetItem(K key, V value) {
            (Node newRoot, bool replaced, _) = root.Set(keyComparer, valComparer, key, value);

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
                return new(Node.Empty, 0, keyComparer, valComparer)
                    .AddRange(this, overwrite: false);
            }
        }

        public Map<K, V> WithDefaultComparers()
            => WithComparers(Comparer<K>.Default, EqualityComparer<V>.Default);

        private Map<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> items, bool overwrite, bool avoidMap) {
            //not in terms of Add so no need for new wrapper per item
            //var result = _root;
            //var count = _count;
            //foreach (var item in items) {
            //    bool mutated;
            //    bool replacedExistingValue = false;
            //    var newResult = overwriteOnCollision
            //        ? result.SetItem(item.Key, item.Value, _keyComparer, _valueComparer, out replacedExistingValue, out mutated)
            //        : result.Add(item.Key, item.Value, _keyComparer, _valueComparer, out mutated);
            //    if (mutated) {
            //        result = newResult;
            //        if (!replacedExistingValue) {
            //            count++;
            //        }
            //    }
            //}

            //return this.Wrap(result, count);

            int count = 0;
            items.Aggregate((root, count), (acc, item) => {
                var newAcc = overwrite
                    ? acc.root.Set(keyComparer, valComparer, item.Key, item.Value)
                    : acc.root.Add(keyComparer, valComparer, item.Key, item.Value)
            });
        }

        private Map<K, V> Wrap(Node _root, int adjustedCount) =>
            _root == root
                ? this
                : _root.IsEmpty
                    ? Clear()
                    : new(_root, adjustedCount, keyComparer, valComparer);

        internal enum KeyCollisionBehaviour {
            SetValue,
            Skip,
            ThrowIfDiff,
            ThrowAlways,
        }
    }
}

