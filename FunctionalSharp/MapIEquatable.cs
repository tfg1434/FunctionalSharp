// ReSharper disable NonReadonlyMemberInGetHashCode

using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public sealed partial class Map<K, V> : IEquatable<Map<K, V>> where K : notnull {
    private int _hashCode;

    /// <summary>
    /// Check map equality using this map's KeyComparer and ValueComparer
    /// </summary>
    /// <param name="other">Map to equate to</param>
    /// <returns>Whether the two maps are equal</returns>
    [Pure]
    public bool Equals(Map<K, V>? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Count != other.Count) return false;
        if (KeyComparer != other.KeyComparer || ValueComparer != other.ValueComparer) return false;
        if (_hashCode != 0 && other._hashCode != 0) return false;

        using Enumerator iterThis = GetEnumerator();
        using Enumerator iterOther = other.GetEnumerator();

        for (int i = 0; i < Count; i++) {
            iterThis.MoveNext();
            iterOther.MoveNext();

            if (KeyComparer.Compare(iterThis.Current.Key, iterOther.Current.Key) != 0) return false;
            if (!ValueComparer.Equals(iterThis.Current.Value, iterOther.Current.Value)) return false;
        }

        return true;
    }

    /// <summary>
    /// Hash code for this map
    /// </summary>
    /// <returns></returns>
    /// <remarks>Uses FNV1A 32-bit hash. Includes comparers in hash</remarks>
    [Pure]
    public override int GetHashCode() {
        if (_hashCode != default) return _hashCode;

        // HashCode hash = new();
        // AsEnumerable().ForEach(item => hash.Add(item));
        // hash.Add(KeyComparer);
        // hash.Add(ValComparer);
        //
        // return _hashCode = hash.ToHashCode();
            
        unchecked {
            return _hashCode = (FNV1A.Hash(AsEnumerable()), KeyComparer, ValComparer: ValueComparer).GetHashCode();
        }

        // const int seed = 487;
        // const int modifier = 31;
        //
        // unchecked {
        //     return hashCode = AsEnumerable().Aggregate(seed,
        //         (acc, item) => acc * modifier + item.GetHashCode());
        // }
    }

    [Pure]
    public static bool operator ==(Map<K, V> self, Map<K, V> other)
        => self.Equals(other);

    [Pure]
    public static bool operator !=(Map<K, V> self, Map<K, V> other)
        => !(self == other);

    [Pure]
    public override bool Equals(object? obj)
        => Equals(obj as Map<K, V>);
}