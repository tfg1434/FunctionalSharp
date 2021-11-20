using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace FunctionalSharp {
    //only use with immutable enumerables since it caches
    //enumerable into an array for display
    class ImmutableEnumerableDebuggerProxy<T> {
        private readonly IEnumerable<T> _enumerable;
        private T[]? _cachedContents;

        public ImmutableEnumerableDebuggerProxy(IEnumerable<T> enumerable)
            => _enumerable = enumerable;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Contents => _cachedContents ??= _enumerable.ToArray();
    }

    sealed class ImmutableMapDebuggerProxy<K, V> : ImmutableEnumerableDebuggerProxy<(K Key, V Value)> 
        where K : notnull {

        public ImmutableMapDebuggerProxy(IEnumerable<(K Key, V Value)> map) : base(map) { }
    }
}