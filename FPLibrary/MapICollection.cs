using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : notnull {
        #region ICollection Methods

        void ICollection.CopyTo(Array array, int index)
            => root.CopyTo(array, index, Count);

        #endregion

        #region ICollection<KeyValuePair<K, V>> Properties

        bool ICollection<KeyValuePair<K, V>>.IsReadOnly => true;

        #endregion

        #region ICollection<KeyValuePair<K, V>> Methods

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item) => throw new NotSupportedException();

        void ICollection<KeyValuePair<K, V>>.Clear() => throw new NotSupportedException();

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
            => throw new NotSupportedException();

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int index) {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Length < index + Count) throw new ArgumentOutOfRangeException(nameof(index));

            foreach ((K k, V v) in this)
                array[index++] = new(k, v);
        }

        #endregion

        #region ICollection Properties

        bool ICollection.IsSynchronized => true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object ICollection.SyncRoot => this;

        #endregion
    }
}