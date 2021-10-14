using System.Collections;
using System.Collections.Generic;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : notnull {
        // ReSharper disable once ArrangeTypeMemberModifiers
        internal sealed partial class Node : IEnumerable<(K Key, V Val)> {
            IEnumerator<(K Key, V Val)> IEnumerable<(K Key, V Val)>.GetEnumerator()
                => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public Enumerator GetEnumerator() => new(this);
        }
    }
}