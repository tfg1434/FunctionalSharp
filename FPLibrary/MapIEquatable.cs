using System;
using System.Linq;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace FPLibrary {
    public sealed partial class Map<K, V> : IEquatable<Map<K, V>> where K : notnull {
        private int hashCode = 0;

        //FNV-1a 32-bit hash
        public override int GetHashCode() {
            if (hashCode != 0) return hashCode;

            var hash = new HashCode();
            AsEnumerable().ForEach(item => hash.Add(item));
            hash.Add(keyComparer);
            hash.Add(valComparer);

            return hashCode = hash.ToHashCode();

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

        public bool Equals(Map<K, V>? other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Count != other.Count) return false;
            if (KeyComparer != other.KeyComparer || ValComparer != other.ValComparer) return false;
            if (hashCode != 0 && other.hashCode != 0) return false;

            using var iterThis = GetEnumerator();
            using var iterOther = other.GetEnumerator();

            for (int i = 0; i < Count; i++) {
                iterThis.MoveNext();
                iterOther.MoveNext();
                
                if (keyComparer.Compare(iterThis.Current.Key, iterOther.Current.Key) != 0) return false;
                if (!valComparer.Equals(iterThis.Current.Val, iterOther.Current.Val)) return false;
            }

            return true;
        }
    }
}