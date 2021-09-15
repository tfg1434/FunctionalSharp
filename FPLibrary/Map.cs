using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

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

        public Map<K, V> WithComparers(IComparer<K> _keyComparer,
            IEqualityComparer<V> _valComparer) {

            if (_keyComparer == keyComparer) {
                //if keyComparer is same but val is not
                return _valComparer == valComparer
                    ? this
                    : new(root, count, keyComparer, _valComparer);
            }
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

