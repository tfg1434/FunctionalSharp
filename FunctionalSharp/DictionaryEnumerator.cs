using System;
using System.Collections;
using System.Collections.Generic;

namespace FunctionalSharp {
    class DictionaryEnumerator<K, V> : IDictionaryEnumerator where K : notnull {
        private readonly IEnumerator<(K Key, V Value)> inner;

        internal DictionaryEnumerator(IEnumerator<(K Key, V Value)> inner) {
            if (inner is null) throw new ArgumentNullException(nameof(inner));

            this.inner = inner;
        }

        public object Current => Entry;
        public DictionaryEntry Entry => new(inner.Current.Key, inner.Current.Value);
        public object Key => inner.Current.Key;
        public object? Value => inner.Current.Value;

        public bool MoveNext() => inner.MoveNext();

        public void Reset() => inner.Reset();
    }
}