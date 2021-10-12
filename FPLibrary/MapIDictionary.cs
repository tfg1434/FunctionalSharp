using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static FPLibrary.F;

namespace FPLibrary {
    public sealed partial class Map<K, V> : IDictionary, IDictionary<K, V> where K : notnull {
        #region IDictionary<K, V> Properties

        ICollection<K> IDictionary<K, V>.Keys => new MapKeysCollectionAccessor<K, V>(this);
        ICollection<V> IDictionary<K, V>.Values => new MapValsCollectionAccessor<K, V>(this);
        
        V IDictionary<K, V>.this[K key] {
            get => this[key];
            set => throw new NotSupportedException();
        }
        
        #endregion
        
        #region IDictionary<K, V> Methods
        
        void IDictionary<K, V>.Add(K key, V val) => throw new NotSupportedException();
        
        bool IDictionary<K, V>.Remove(K key) => throw new NotSupportedException();
        
        #endregion
        
        #region IDictionary Properties
        
        bool IDictionary.IsFixedSize => true;
        
        bool IDictionary.IsReadOnly => true;

        ICollection IDictionary.Keys => new MapKeysCollectionAccessor<K, V>(this);

        ICollection IDictionary.Values => new MapValsCollectionAccessor<K, V>(this);
        
        #endregion
        
        #region IDictionary Methods
        
        bool IDictionary.Contains(object key) => ContainsKey((K) key);
        
        void IDictionary.Add(object key, object? value) => throw new NotSupportedException();
        
        void IDictionary.Clear() => throw new NotSupportedException();
        
        void IDictionary.Remove(object key) => throw new NotSupportedException();
        
        object? IDictionary.this[object key] {
            get => this[(K) key];
            set => throw new NotSupportedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
            => new DictionaryEnumerator<K, V>(GetEnumerator());

        #endregion
    }
}