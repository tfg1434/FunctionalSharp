using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FPLibrary {
    //only use with immutable enumerables since it caches
    //enumerable into an array for display
    class ImmutableEnumerableDebuggerProxy<T> {
        protected readonly IEnumerable<T> enumerable;
        private T[]? cachedContents;

        public ImmutableEnumerableDebuggerProxy(IEnumerable<T> enumerable)
            => this.enumerable = enumerable;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Contents => cachedContents ??= enumerable.ToArray();
    }
}