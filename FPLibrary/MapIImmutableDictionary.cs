using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FPLibrary {
    public sealed partial class Map<K, V> : IImmutableDictionary<K, V> where K : notnull {
        #region Methods

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Add(K key, V val) => Add((key, val));

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.AddRange(IEnumerable<KeyValuePair<K, V>> items)
            => AddRange(items.Map(x => (x.Key, x.Value)));

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Clear() => Clear();

        bool IImmutableDictionary<K, V>.Contains(KeyValuePair<K, V> pair)
            => Contains((pair.Key, pair.Value));

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Remove(K key) => Remove(key);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.RemoveRange(IEnumerable<K> keys)
            => RemoveRange(keys);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItem(K key, V value) => SetItem((key, value));

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItems(IEnumerable<KeyValuePair<K, V>> items)
            => SetItems(items);

        #endregion
    }
}