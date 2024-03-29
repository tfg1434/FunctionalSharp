﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace FunctionalSharp; 

//make inner key/value enumerable an ICollection
abstract class MapKeysValuesCollectionAccessor<K, V, T> : ICollection<T>, ICollection where K : notnull {
    private readonly IEnumerable<T> _innerEnumerable;

    protected MapKeysValuesCollectionAccessor(IImmutableDictionary<K, V> map, IEnumerable<T> enumerable) {
        Map = map;
        _innerEnumerable = enumerable;
    }

    protected IImmutableDictionary<K, V> Map { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection.IsSynchronized => true;

    object ICollection.SyncRoot => this;

    void ICollection.CopyTo(Array array, int arrayIndex) {
        if (array is null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length < arrayIndex + Count) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        foreach (T item in this)
            array.SetValue(item, arrayIndex++);
    }

    public bool IsReadOnly => true;
    public int Count => Map.Count;

    public IEnumerator<T> GetEnumerator() => _innerEnumerable.GetEnumerator();

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
}

sealed class MapKeysCollectionAccessor<K, V> : MapKeysValuesCollectionAccessor<K, V, K> where K : notnull {
    internal MapKeysCollectionAccessor(IImmutableDictionary<K, V> map)
        : base(map, map.Keys) { }

    public override bool Contains(K item) => Map.ContainsKey(item);
}

sealed class MapValuesCollectionAccessor<K, V> : MapKeysValuesCollectionAccessor<K, V, V> where K : notnull {
    internal MapValuesCollectionAccessor(IImmutableDictionary<K, V> map)
        : base(map, map.Values) { }

    public override bool Contains(V item) => (Map as Map<K, V>)!.ContainsValue(item);
}