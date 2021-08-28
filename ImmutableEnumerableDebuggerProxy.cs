using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace FPLibrary {
    using static F;

    //only use with immutable enumerables since it caches
    //enumerable into an array for display
    internal class ImmutableEnumerableDebuggerProxy<T> {
        protected readonly IEnumerable<T> enumerable;
        private T[]? cachedContents;

        public ImmutableEnumerableDebuggerProxy(IEnumerable<T> enumerable)
            => this.enumerable = enumerable;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Contents => cachedContents ??= enumerable.ToArray();
    }
}
