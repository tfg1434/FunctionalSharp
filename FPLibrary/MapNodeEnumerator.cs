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
        internal sealed partial class Node : IEnumerable<(K Key, V Val)> {
            public Enumerator GetEnumerator() => new(this);

            IEnumerator<(K Key, V Val)> IEnumerable<(K Key, V Val)>.GetEnumerator()
                => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
