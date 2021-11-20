using System;
using System.Collections.Generic;

namespace FunctionalSharp {
    public sealed partial class Map<K, V> where K : notnull {
        public V this[K key] => Get(key)
            .IfNothing(() => throw new ArgumentException("Key does not exist in map.", nameof(key)));

        public IEnumerable<K> Keys => _root.Keys;
        public IEnumerable<V> Values => _root.Values;
    }
}