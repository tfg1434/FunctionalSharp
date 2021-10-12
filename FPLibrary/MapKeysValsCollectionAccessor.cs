using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace FPLibrary {
    //make inner key/value enumerable an ICollection
    internal abstract class MapKeysValsCollectionAccessor<K, V, T> : ICollection<T>, ICollection where K : notnull {
        private readonly IImmutableDictionary<K, V> innerMap;
        private readonly IEnumerable<T> innerEnumerable;

        protected MapKeysValsCollectionAccessor(IImmutableDictionary<K, V> map, IEnumerable<T> enumerable) {
            innerMap = map ?? throw new ArgumentNullException(nameof(enumerable));
            innerEnumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
        }

        public bool IsReadOnly => true;
        public int Count => innerMap.Count;
        protected IImmutableDictionary<K, V> Map => innerMap;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => this;

        public IEnumerator<T> GetEnumerator() => innerEnumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public void Add(T item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public abstract bool Contains(T item);
        
        public void CopyTo(T[] array, int arrayIndex) {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length < arrayIndex + Count) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            foreach (T item in this)
                array[arrayIndex++] = item;
        }

        public bool Remove(T item) => throw new NotSupportedException();

        void ICollection.CopyTo(Array array, int arrayIndex) {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length < arrayIndex + Count) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            
            foreach (T item in this)
                array.SetValue(item, arrayIndex++);
        }
    }

    internal sealed class MapKeysCollectionAccessor<K, V> : MapKeysValsCollectionAccessor<K, V, K> where K : notnull {
        internal MapKeysCollectionAccessor(IImmutableDictionary<K, V> map) 
            : base(map, map.Keys) { }

        public override bool Contains(K item) => Map.ContainsKey(item);
    }

    internal sealed class MapValsCollectionAccessor<K, V> : MapKeysValsCollectionAccessor<K, V, V> where K : notnull {
        internal MapValsCollectionAccessor(IImmutableDictionary<K, V> map)
            : base(map, map.Values) { }

        public override bool Contains(V item) => (Map as Map<K, V>)!.ContainsVal(item);
    }
}