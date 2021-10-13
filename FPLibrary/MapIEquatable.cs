using System;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace FPLibrary {
    public sealed partial class Map<K, V> : IEquatable<Map<K, V>> where K : notnull {
        private int _hashCode;

        public bool Equals(Map<K, V>? other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Count != other.Count) return false;
            if (KeyComparer != other.KeyComparer || ValComparer != other.ValComparer) return false;
            if (_hashCode != 0 && other._hashCode != 0) return false;

            using Enumerator iterThis = GetEnumerator();
            using Enumerator iterOther = other.GetEnumerator();

            for (int i = 0; i < Count; i++) {
                iterThis.MoveNext();
                iterOther.MoveNext();

                if (KeyComparer.Compare(iterThis.Current.Key, iterOther.Current.Key) != 0) return false;
                if (!ValComparer.Equals(iterThis.Current.Val, iterOther.Current.Val)) return false;
            }

            return true;
        }

        //FNV-1a 32-bit hash
        public override int GetHashCode() {
            if (_hashCode != default) return _hashCode;

            HashCode hash = new();
            AsEnumerable().ForEach(item => hash.Add(item));
            hash.Add(KeyComparer);
            hash.Add(ValComparer);

            return _hashCode = hash.ToHashCode();

            // int hash = -2128831035;
            //
            // unchecked {
            //     return (hashCode = AsEnumerable().Aggregate(hash, 
            //             (acc, item) => (item.GetHashCode() ^ acc) * 16777619), 
            //         keyComparer, valComparer).GetHashCode();
            // }

            // const int seed = 487;
            // const int modifier = 31;
            //
            // unchecked {
            //     return hashCode = AsEnumerable().Aggregate(seed,
            //         (acc, item) => acc * modifier + item.GetHashCode());
            // }
        }

        public static bool operator ==(Map<K, V> self, Map<K, V> other)
            => self.Equals(other);

        public static bool operator !=(Map<K, V> self, Map<K, V> other)
            => !(self == other);

        public override bool Equals(object? obj)
            => Equals(obj as Map<K, V>);
    }
}