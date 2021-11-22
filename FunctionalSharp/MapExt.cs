using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public static partial class F {
    #region Map

    /// <summary>
    /// Construct a Map
    /// </summary>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map</returns>
    [Pure]
    public static Map<K, V> Map<K, V>(params KeyValuePair<K, V>[] items) where K : notnull
        => Map((IEnumerable<KeyValuePair<K, V>>) items);

    /// <summary>
    /// Construct a Map
    /// </summary>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map</returns>
    [Pure]
    public static Map<K, V> Map<K, V>(params Tuple<K, V>[] items) where K : notnull
        => Map((IEnumerable<Tuple<K, V>>) items);

    /// <summary>
    /// Construct a Map
    /// </summary>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map</returns>
    [Pure]
    public static Map<K, V> Map<K, V>(IEnumerable<KeyValuePair<K, V>> items) where K : notnull
        => Map<K, V>().AddRange(items);

    /// <summary>
    /// Construct a Map
    /// </summary>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map</returns>
    [Pure]
    public static Map<K, V> Map<K, V>(IEnumerable<Tuple<K, V>> items) where K : notnull
        => Map<K, V>().AddRange(items);

    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
        IEqualityComparer<V>? valComparer = null, params KeyValuePair<K, V>[] items) where K : notnull
        => MapWithComparers(keyComparer, valComparer).AddRange(items);

    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
        IEqualityComparer<V>? valComparer = null, params Tuple<K, V>[] items) where K : notnull
        => MapWithComparers(keyComparer, valComparer).AddRange(items);

    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IEnumerable<KeyValuePair<K, V>> items,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull
        => MapWithComparers(keyComparer, valComparer).AddRange(items);

    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IEnumerable<Tuple<K, V>> items,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull
        => MapWithComparers(keyComparer, valComparer).AddRange(items);

    #endregion

    #region ToMap
    
    /// <summary>
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with no projections and default comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <returns>Map with default comparers</returns>
    [Pure]
    public static Map<K, V> ToMap<K, V>(this IEnumerable<KeyValuePair<K, V>> src) where K : notnull
        => Map(src);

    /// <summary>
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with no projections and default comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <returns>Map with default comparers</returns>
    [Pure]
    public static Map<K, V> ToMap<K, V>(this IEnumerable<Tuple<K, V>> src) where K : notnull
        => Map(src);

    /// <summary>
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with no projections but custom comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <returns>Map with custom comparers</returns>
    [Pure]
    public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<KeyValuePair<K, V>> src,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull {
        if (src is null) throw new ArgumentNullException(nameof(src));

        return MapWithComparers(keyComparer, valComparer).AddRange(src);
    }

    /// <summary>
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with no projections but custom comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <returns>Map with custom comparers</returns>
    [Pure]
    public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<Tuple<K, V>> src,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull {
        if (src is null) throw new ArgumentNullException(nameof(src));

        return MapWithComparers(keyComparer, valComparer)
            .AddRange(src);
    }

    #endregion
}

public sealed partial class Map<K, V> where K : notnull {
    /// <summary>
    /// See if map contains a <see cref="KeyValuePair{TKey,TValue}"/>
    /// </summary>
    /// <param name="kv"><see cref="KeyValuePair{TKey,TValue}"/> to search for</param>
    /// <returns>Whether map contains this <see cref="KeyValuePair{TKey,TValue}"/></returns>
    [Pure]
    public bool Contains(KeyValuePair<K, V> kv)
        => Contains(ToValueTuple(kv));

    /// <summary>
    /// Add a <paramref name="key"/> and associated <paramref name="value"/> to the map
    /// </summary>
    /// <param name="key">Key to add</param>
    /// <param name="value">Value to add</param>
    /// <returns>New map with key and value added</returns>
    [Pure]
    public Map<K, V> Add(K key, V value) => Add((key, value));

    /// <summary>
    /// Add a pair to the map
    /// </summary>
    /// <param name="kv">Pair to add</param>
    /// <returns>New map with pair added</returns>
    [Pure]
    public Map<K, V> Add(KeyValuePair<K, V> kv) => Add(ToValueTuple(kv));

    /// <summary>
    /// Add a pair to the map
    /// </summary>
    /// <param name="tup">Pair to add</param>
    /// <returns>New map with pair added</returns>
    [Pure]
    public Map<K, V> Add(Tuple<K, V> tup) => Add(ToValueTuple(tup));

    public Map<K, V> AddRange(params (K, V)[] items)
        => AddRange((IEnumerable<(K Key, V Value)>) items);

    public Map<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> items)
        => AddRange(items.Map(ToValueTuple));

    public Map<K, V> AddRange(params KeyValuePair<K, V>[] items)
        => AddRange(items.Map(ToValueTuple));

    public Map<K, V> AddRange(IEnumerable<Tuple<K, V>> items)
        => AddRange(items.Map(ToValueTuple));

    public Map<K, V> AddRange(params Tuple<K, V>[] items)
        => AddRange(items.Map(ToValueTuple));

    public bool Contains(K key, V val)
        => Contains((key, val));

    public bool Contains(Tuple<K, V> tup)
        => Contains(ToValueTuple(tup));

    public Map<K, V> RemoveRange(params K[] items)
        => RemoveRange((IEnumerable<K>) items);

    public Map<K, V> SetItem(K key, V val) => SetItem((key, val));

    public Map<K, V> SetItem(KeyValuePair<K, V> kv) => SetItem(ToValueTuple(kv));

    public Map<K, V> SetItem(Tuple<K, V> tup) => SetItem(ToValueTuple(tup));

    public Map<K, V> SetItems(params (K Key, V Value)[] items)
        => SetItems((IEnumerable<(K Key, V Value)>) items);

    public Map<K, V> SetItems(params KeyValuePair<K, V>[] items)
        => SetItems(items.Map(ToValueTuple));

    public Map<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
        => SetItems(items.Map(ToValueTuple));

    public Map<K, V> SetItems(Tuple<K, V>[] items)
        => SetItems(items.Map(ToValueTuple));

    public Map<K, V> SetItems(IEnumerable<Tuple<K, V>> items)
        => SetItems(items.Map(ToValueTuple));
}