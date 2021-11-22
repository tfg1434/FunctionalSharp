using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public sealed partial class Map<K, V> : IImmutableDictionary<K, V> where K : notnull {
    #region IImmutableDictionary

    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Add(K key, V value) 
        => Append((key, value));

    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.AddRange(IEnumerable<KeyValuePair<K, V>> items)
        => Append(items.Map(x => (x.Key, x.Value)));

    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Clear() 
        => Clear();

    bool IImmutableDictionary<K, V>.Contains(KeyValuePair<K, V> pair)
        => Contains((pair.Key, pair.Value));

    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Remove(K key) => Remove(key);

    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.RemoveRange(IEnumerable<K> keys)
        => RemoveRange(keys);

    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItem(K key, V value) => SetItem((key, value));

    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItems(IEnumerable<KeyValuePair<K, V>> items)
        => SetItems(items);
    
    #endregion

    #region IReadonlyDictionary

    /// <summary>
    /// Unsafely get a value from the map
    /// </summary>
    /// <param name="key">Key to get from</param>
    /// <exception cref="KeyNotFoundException">If key does not exist in the map</exception>
    [Pure]
    public V this[K key] => Get(key).IfNothing(
        () => throw new KeyNotFoundException("Key does not exist in map."));

    /// <summary>
    /// IEnumerable of the keys in the map
    /// </summary>
    [Pure]
    public IEnumerable<K> Keys => _root.Keys;
        
    /// <summary>
    /// IEnumerable of the values in the map
    /// </summary>
    [Pure]
    public IEnumerable<V> Values => _root.Values;

    #endregion

    #region ICollection

    void ICollection.CopyTo(Array array, int index)
        => _root.CopyTo(array, index, Count);
    
    bool ICollection<KeyValuePair<K, V>>.IsReadOnly => true;
    
    void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item) => throw new NotSupportedException();

    void ICollection<KeyValuePair<K, V>>.Clear() => throw new NotSupportedException();

    bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        => throw new NotSupportedException();

    void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int index) {
        if (array is null) throw new ArgumentNullException(nameof(array));
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        if (array.Length < index + Count) throw new ArgumentOutOfRangeException(nameof(index));

        foreach ((K k, V v) in this)
            array[index++] = new(k, v);
    }
    
    bool ICollection.IsSynchronized => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object ICollection.SyncRoot => this;

    #endregion
    
}