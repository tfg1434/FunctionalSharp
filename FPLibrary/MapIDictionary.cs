using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> : IDictionary, IDictionary<K, V> where K : notnull {
        #region IDictionary<K, V> Properties

        ICollection<K> IDictionary<K, V>.Keys => throw new NotImplementedException();
        ICollection<V> IDictionary<K, V>.Values => throw new NotImplementedException();

        V IDictionary<K, V>.this[K key] {
            get => this[key];
            set => throw new NotSupportedException();
        }

        #endregion

        #region IDictionary<K, V> Methods

        void IDictionary<K, V>.Add(K key, V val) => throw new NotImplementedException();
        bool IDictionary<K, V>.Remove(K key) => throw new NotImplementedException();

        #endregion

        #region IDictionary Properties

        bool IDictionary.IsFixedSize => true;
        

        #endregion

        #region IDictionary Methods



        #endregion
    }
}