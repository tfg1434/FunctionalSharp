using System.Collections;
using System.Collections.Generic;

namespace FunctionalSharp {
    public sealed partial class Map<K, V> where K : notnull {
        // ReSharper disable once ArrangeTypeMemberModifiers
        internal sealed partial class Node : IEnumerable<(K Key, V Value)> {
            IEnumerator<(K Key, V Value)> IEnumerable<(K Key, V Value)>.GetEnumerator()
                => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public Enumerator GetEnumerator() => new(this);
        }
    }
}