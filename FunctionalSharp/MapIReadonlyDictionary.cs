using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp {
    public sealed partial class Map<K, V> where K : notnull {
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
    }
}