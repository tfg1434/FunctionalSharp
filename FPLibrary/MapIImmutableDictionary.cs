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
        public IImmutableDictionary<K, V> Add(K key, V val)
            => root
                .Add(keyComparer, valComparer, key, val).Node
                .Pipe(
                    Wrap
                        .Flip()
                        .Apply(count + 1));
        IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Clear() => Clear();

        #endregion
        

        // IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() => throw new NotImplementedException();
        //
        // IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        //
        // void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item) {
        //     throw new NotImplementedException();
        // }
        //
        // public IImmutableDictionary<K, V> Add(K key, V value) => throw new NotImplementedException();
        //
        // bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item) => throw new NotImplementedException();
        // public IImmutableDictionary<K, V> Remove(K key) => throw new NotImplementedException();
        //
        // public IImmutableDictionary<K, V> RemoveRange(IEnumerable<K> keys) => throw new NotImplementedException();
        //
        // public IImmutableDictionary<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items) => throw new NotImplementedException();
        //
        // public bool TryGetKey(K equalKey, out K actualKey) => throw new NotImplementedException();
        //
        // void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
        //     throw new NotImplementedException();
        // }
        //
        // bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item) => throw new NotImplementedException();
        //
        // int ICollection<KeyValuePair<K, V>>.Count => count1;
        //
        // bool ICollection<KeyValuePair<K, V>>.IsReadOnly => isReadOnly;
        //
        // void IDictionary<K, V>.Add(K key, V value) {
        //     throw new NotImplementedException();
        // }
        //
        // bool IDictionary<K, V>.ContainsKey(K key) => throw new NotImplementedException();
        // public bool TryGetValue(K key, out V value) => throw new NotImplementedException();
        //
        // public V this[K key] => throw new NotImplementedException();
        //
        // public IEnumerable<K> Keys { get; }
        // public IEnumerable<V> Values { get; }
        //
        // public bool Contains(KeyValuePair<K, V> pair) => throw new NotImplementedException();
        //
        // bool IDictionary<K, V>.Remove(K key) => throw new NotImplementedException();
        //
        // public bool ContainsKey(K key) => throw new NotImplementedException();
        //
        // bool IDictionary<K, V>.TryGetValue(K key, out V value) => throw new NotImplementedException();
        //
        // V IDictionary<K, V>.this[K key] {
        //     get => throw new NotImplementedException();
        //     set => throw new NotImplementedException();
        // }
        //
        // ICollection<K> IDictionary<K, V>.Keys => keys;
        //
        // ICollection<V> IDictionary<K, V>.Values => values;
        // public int Count { get; }
        //
        // IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Clear() => Clear();
        //
        // void ICollection<KeyValuePair<K, V>>.Clear() => throw new NotSupportedException();
        //
        // IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItem(K key, V val) => SetItem(key, val);
        //
        // IImmutableDictionary<K, V> IImmutableDictionary<K, V>.AddRange(IEnumerable<KeyValuePair<K, V>> pairs) 
        //     => AddRange(pairs);
    }
}