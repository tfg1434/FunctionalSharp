using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FPLibrary;
using static FPLibrary.F;
using System.Linq;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : notnull {
        internal sealed partial class Node : IEnumerable<KeyValuePair<K, V>> {
            public Enumerator GetEnumerator() => new(this);

            IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
                => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
