using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> : IImmutableDictionary<K, V> where K : notnull {
        #region Properties
        
        public int Count => count;
        public IEnumerable<K> Keys => throw new NotImplementedException();
        public IEnumerable<V> Values => throw new NotImplementedException();
        public V this[K key] => throw new NotImplementedException();

        #endregion

        #region Methods

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Add(K key, V val) => Add(key, val);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.AddRange(IEnumerable<KeyValuePair<K, V>> items) 
            => AddRange(items);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Clear() => Clear();

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Remove(K key) => throw new NotImplementedException();
        
        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.RemoveRange(IEnumerable<K> keys) 
            => throw new NotImplementedException();

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItem(K key, V value) => SetItem(key, value);

        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItems(IEnumerable<KeyValuePair<K, V>> items)
            => throw new NotImplementedException();

        #endregion
    }
}