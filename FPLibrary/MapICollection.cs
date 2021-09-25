using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> where K : notnull {
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

            foreach (KeyValuePair<K, V> item in this)
                array[index++] = item;
        }

        #endregion
        
        #region ICollection Properties

        bool ICollection.IsSynchronized => true;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object ICollection.SyncRoot => this;

        #endregion

        #region ICollection Methods

        void ICollection.CopyTo(Array array, int index) 
            => root.CopyTo(array, index, Count);

        #endregion
    }
}