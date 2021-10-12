using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : notnull {
        public V this[K key] => Get(key)
            .IfNothing(() => throw new ArgumentException("Key does not exist in map.", nameof(key)));

        public IEnumerable<K> Keys => root.Keys;
        public IEnumerable<V> Values => root.Values;
    }
}