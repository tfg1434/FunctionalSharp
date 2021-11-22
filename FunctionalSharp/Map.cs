using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FunctionalSharp; 

public static partial class F {
    #region Map

    /// <summary>
    /// Construct an empty Map
    /// </summary>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <returns>Empty Map</returns>
    [Pure]
    public static Map<K, V> Map<K, V>() where K : notnull
        => FunctionalSharp.Map<K, V>.Empty;
        
    /// <summary>
    /// Construct a Map
    /// </summary>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map</returns>
    [Pure]
    public static Map<K, V> Map<K, V>(params (K Key, V Value)[] items) where K : notnull
        => Map((IEnumerable<(K Key, V Value)>) items);
    
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
    public static Map<K, V> Map<K, V>(IEnumerable<(K Key, V Value)> items) where K : notnull
        => Map<K, V>().Append(items);
    
    /// <summary>
    /// Construct a Map
    /// </summary>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map</returns>
    [Pure]
    public static Map<K, V> Map<K, V>(IEnumerable<KeyValuePair<K, V>> items) where K : notnull
        => Map<K, V>().Append(items);

    /// <summary>
    /// Construct a Map
    /// </summary>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map</returns>
    [Pure]
    public static Map<K, V> Map<K, V>(IEnumerable<Tuple<K, V>> items) where K : notnull
        => Map<K, V>().Append(items);

    #endregion
    
    #region MapWithComparers
    
    /// <summary>
    /// Construct an empty Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <typeparam name="K">Type of keys in map</typeparam>
    /// <typeparam name="V">Type of values in maps</typeparam>
    /// <returns>Empty map with comparers</returns>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
        IEqualityComparer<V>? valueComparer = null) where K : notnull
        => FunctionalSharp.Map<K, V>.Empty.WithComparers(keyComparer, valueComparer);
    
    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
        IEqualityComparer<V>? valueComparer = null, params (K Key, V Value)[] items) where K : notnull
        => MapWithComparers(keyComparer, valueComparer).Append(items);

    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
        IEqualityComparer<V>? valueComparer = null, params KeyValuePair<K, V>[] items) where K : notnull
        => MapWithComparers(keyComparer, valueComparer).Append(items);
    
    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
        IEqualityComparer<V>? valueComparer = null, params Tuple<K, V>[] items) where K : notnull
        => MapWithComparers(keyComparer, valueComparer).Append(items);

    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    /// <remarks>Parameters are in this order because the comparers are optional.</remarks>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IEnumerable<(K Key, V Value)> items,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valueComparer = null) where K : notnull
        => MapWithComparers(keyComparer, valueComparer).Append(items);

    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    /// <remarks>Parameters are in this order because the comparers are optional.</remarks>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IEnumerable<KeyValuePair<K, V>> items,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valueComparer = null) where K : notnull
        => MapWithComparers(keyComparer, valueComparer).Append(items);
    
    /// <summary>
    /// Construct a Map with custom comparers
    /// </summary>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <param name="items">Pairs to construct map from</param>
    /// <typeparam name="K">Type of keys in pair</typeparam>
    /// <typeparam name="V">Type of values in pair</typeparam>
    /// <returns>Created map with comparers</returns>
    /// <remarks>Parameters are in this order because the comparers are optional.</remarks>
    [Pure]
    public static Map<K, V> MapWithComparers<K, V>(IEnumerable<Tuple<K, V>> items,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valueComparer = null) where K : notnull
        => MapWithComparers(keyComparer, valueComparer).Append(items);
    
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
    public static Map<K, V> ToMap<K, V>(this IEnumerable<(K Key, V Value)> src) where K : notnull
        => Map(src);
    
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
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with both key and value projections and default comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <param name="keyProj">Key projection function</param>
    /// <param name="valProj">Value projection function</param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <typeparam name="T">Type of IEnumerable to convert</typeparam>
    /// <returns>Map with default comparers</returns>
    [Pure]
    public static Map<K, V> ToMap<T, K, V>(this IEnumerable<T> src, Func<T, K> keyProj, Func<T, V> valProj)
        where K : notnull
        => src.ToMapWithComparers(keyProj, valProj);
    
    #endregion

    #region ToMapWithComparers
    
    /// <summary>
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with no projections but custom comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <returns>Map with custom comparers</returns>
    [Pure]
    public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<(K Key, V Value)> src,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valueComparer = null) where K : notnull {
        if (src is null) throw new ArgumentNullException(nameof(src));

        return MapWithComparers(keyComparer, valueComparer)
            .Append(src);
    }
    
    /// <summary>
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with no projections but custom comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <returns>Map with custom comparers</returns>
    [Pure]
    public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<KeyValuePair<K, V>> src,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valueComparer = null) where K : notnull {
        if (src is null) throw new ArgumentNullException(nameof(src));

        return MapWithComparers(keyComparer, valueComparer).Append(src);
    }

    /// <summary>
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with no projections but custom comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <returns>Map with custom comparers</returns>
    [Pure]
    public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<Tuple<K, V>> src,
        IComparer<K>? keyComparer = null, IEqualityComparer<V>? valueComparer = null) where K : notnull {
        if (src is null) throw new ArgumentNullException(nameof(src));

        return MapWithComparers(keyComparer, valueComparer)
            .Append(src);
    }

    /// <summary>
    /// Convert an <see cref="IEnumerable{T}"/> to a Map with both projections and custom comparers
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to convert</param>
    /// <param name="keyProj">Key projection function</param>
    /// <param name="valProj">Value projection function</param>
    /// <param name="keyComparer">KeyComparer. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">ValueComparer. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <typeparam name="K">Type of keys</typeparam>
    /// <typeparam name="V">Type of values</typeparam>
    /// <typeparam name="T">Type of IEnumerable to convert</typeparam>
    /// <returns>Map with custom comparers</returns>
    [Pure]
    public static Map<K, V> ToMapWithComparers<T, K, V>(this IEnumerable<T> src, Func<T, K> keyProj,
        Func<T, V> valProj, IComparer<K>? keyComparer = null, IEqualityComparer<V>? valueComparer = null)
        where K : notnull
        => src
            .Map(t => (keyProj(t), valProj(t)))
            .ToMapWithComparers(keyComparer, valueComparer);
    
    #endregion
}

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ImmutableMapDebuggerProxy<,>))]
public sealed partial class Map<K, V> where K : notnull {
    private readonly Node _root;

    #region Ctors

    //default comparer overload
    private Map() => _root = Node.EmptyNode;

    private Map(Node root, int count) {
        root.Freeze();
        _root = root;
        Count = count;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Number of elements in this map
    /// </summary>
    [Pure]
    public int Count { get; }
    
    /// <summary>
    /// Whether this map is empty
    /// </summary>
    [Pure]
    public bool IsEmpty => Count == 0;
    
    /// <summary>
    /// The key comparer used by this map
    /// </summary>
    [Pure]
    public IComparer<K> KeyComparer { get; init; } = Comparer<K>.Default;
    
    /// <summary>
    /// The value comparer used by this map
    /// </summary>
    [Pure]
    public IEqualityComparer<V> ValueComparer { get; init; } = EqualityComparer<V>.Default;
    
    /// <summary>
    /// The empty Map with default comparers.
    /// </summary>
    [Pure]
    public static Map<K, V> Empty => new();

    #endregion

    //TODO: See Lst concats
    [Pure]
    public static Map<K, V> operator +(Map<K, V> lhs, Map<K, V> rhs)
        => lhs.Append(rhs);

    /// <summary>
    /// Give this map custom comparers
    /// </summary>
    /// <param name="keyComparer">Key comparer to use. If null, uses <see cref="Comparer{T}"/></param>
    /// <param name="valueComparer">Value comparer to use. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <returns>New map with custom comparers</returns>
    public Map<K, V> WithComparers(IComparer<K>? keyComparer = null, IEqualityComparer<V>? valueComparer = null) {
        keyComparer ??= Comparer<K>.Default;
        valueComparer ??= EqualityComparer<V>.Default;

        if (keyComparer == KeyComparer) //structure doesn't depend on valueComparer, so just one new node
            return valueComparer == ValueComparer
                ? this
                : new(_root, Count) { KeyComparer = KeyComparer, ValueComparer = valueComparer };

        return new Map<K, V>(Node.EmptyNode, 0) { KeyComparer = keyComparer, ValueComparer = valueComparer }
            .Append(this, false, true);
    }

    //TODO: avoidMap
    private Map<K, V> Append(IEnumerable<(K Key, V Value)> items, bool overwrite, bool avoidMap) {
        if (items is null) throw new ArgumentNullException(nameof(items));
        
        //not in terms of Add so no need for new wrapper per item

        (Node Root, int Count) seed = (_root, Count);
        (Node Root, int Count) res = items.Aggregate(seed, (acc, item) => {
            Node newRes;
            bool replaced = false;
            bool mutated;
            if (overwrite)
                (newRes, replaced, mutated) = acc.Root.Set(KeyComparer, ValueComparer, item.Key, item.Value);
            else
                (newRes, mutated) = acc.Root.Add(KeyComparer, ValueComparer, item.Key, item.Value);

            return mutated ? (newRes, replaced ? acc.Count : acc.Count + 1) : (acc.Root, acc.Count);
        });

        return Wrap(res.Root, res.Count);
    }
        
    private Func<Node, int, Map<K, V>> Wrap => (root, adjustedCount) => {
        if (_root != root)
            return root.IsEmpty 
                ? Clear() 
                : new(root, adjustedCount) { KeyComparer = KeyComparer, ValueComparer = ValueComparer };

        return this;
    };
    
    private static KeyValuePair<K, V> ToKeyValuePair((K Key, V Value) pair)
        => new(pair.Key, pair.Value);

    private static (K Key, V Value) ToValueTuple(KeyValuePair<K, V> kv)
        => (kv.Key, kv.Value);

    private static (K Key, V Value) ToValueTuple(Tuple<K, V> tup)
        => (tup.Item1, tup.Item2);
}