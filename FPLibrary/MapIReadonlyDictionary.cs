using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> : IReadOnlyDictionary<K, V> where K : notnull {
        public V this[K key] => throw new NotImplementedException();

        public IEnumerable<K> Keys => throw new NotImplementedException();
        public IEnumerable<V> Values => throw new NotImplementedException();
    }
}