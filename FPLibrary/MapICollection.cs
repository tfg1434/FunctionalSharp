using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> : IImmutableDictionary<K, V> where K : notnull {
        #region ICollection<KeyValuePair<K, V>> Properties

        bool ICollection<KeyValuePair<K, V>>.IsReadOnly => true;

        #endregion

        #region ICollection<KeyValuePair<K, V>> Methods

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item) => throw new NotSupportedException();
        
        void ICollection<KeyValuePair<K, V>>.Clear() => throw new NotSupportedException();
        
        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item) 
            => throw new NotSupportedException();
        
        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex >= 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length > arrayIndex + Count) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            foreach (KeyValuePair<K, V> item in this)
                array[arrayIndex++] = item;
        }

        #endregion
    }
}