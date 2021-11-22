using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public sealed partial class Map<K, V> where K : notnull {
    #region Contains
    
    /// <summary>
    /// See if map contains a specific key
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <returns>Whether the map contains <paramref name="key"/></returns>
    [Pure]
    public bool ContainsKey(K key) {
        if (key is null) throw new ArgumentNullException(nameof(key));

        return _root.ContainsKey(KeyComparer, key);
    }
    
    /// <summary>
    /// See if map contains a specific value
    /// </summary>
    /// <param name="value">Value to look for</param>
    /// <returns>Whether the map contains <paramref name="value"/></returns>
    [Pure]
    public bool ContainsValue(V value) 
        => _root.ContainsValue(ValueComparer, value);
    
    /// <summary>
    /// See if map contains a specific key and value
    /// </summary>
    /// <param name="pair">Pair to look for</param>
    /// <returns>Whether or not map contains this pair</returns>
    [Pure]
    public bool Contains((K Key, V Value) pair)
        => _root.Contains(KeyComparer, ValueComparer, pair);
    
    /// <summary>
    /// See if map contains a <see cref="KeyValuePair{TKey,TValue}"/>
    /// </summary>
    /// <param name="kv"><see cref="KeyValuePair{TKey,TValue}"/> to search for</param>
    /// <returns>Whether map contains this <see cref="KeyValuePair{TKey,TValue}"/></returns>
    [Pure]
    public bool Contains(KeyValuePair<K, V> kv)
        => Contains(ToValueTuple(kv));
    
    /// <summary>
    /// See if map contains a specific key and value
    /// </summary>
    /// <param name="tup">Pair to look for</param>
    /// <returns>Whether or not map contains this pair</returns>
    [Pure]
    public bool Contains(Tuple<K, V> tup)
        => Contains(ToValueTuple(tup));
    
    /// <summary>
    /// See if the map contains a specific key and value
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <param name="value">Value to look for</param>
    /// <returns>Whether or not map contains this key and value</returns>
    [Pure]
    public bool Contains(K key, V value) => Contains((key, value));

    #endregion

    #region Append

    /// <summary>
    /// Append a pair to the map
    /// </summary>
    /// <param name="pair">Pair to add</param>
    /// <returns>New map with pair added</returns>
    [Pure]
    public Map<K, V> Append((K Key, V Value) pair) {
        (Node res, _) = _root.Add(KeyComparer, ValueComparer, pair);

        return Wrap(res, Count + 1);
    }
    
    /// <summary>
    /// Append a <paramref name="key"/> and associated <paramref name="value"/> to the map
    /// </summary>
    /// <param name="key">Key to add</param>
    /// <param name="value">Value to add</param>
    /// <returns>New map with key and value added</returns>
    [Pure]
    public Map<K, V> Append(K key, V value) => Append((key, value));

    /// <summary>
    /// Append a pair to the map
    /// </summary>
    /// <param name="kv">Pair to add</param>
    /// <returns>New map with pair added</returns>
    [Pure]
    public Map<K, V> Append(KeyValuePair<K, V> kv) => Append(ToValueTuple(kv));

    /// <summary>
    /// Append a pair to the map
    /// </summary>
    /// <param name="tup">Pair to add</param>
    /// <returns>New map with pair added</returns>
    [Pure]
    public Map<K, V> Append(Tuple<K, V> tup) => Append(ToValueTuple(tup));
    
    /// <summary>
    /// Append an enumerable of pairs to the map
    /// </summary>
    /// <param name="items">Pairs to add</param>
    /// <returns>New map with pairs added</returns>
    [Pure]
    public Map<K, V> Append(IEnumerable<(K Key, V Value)> items)
        => Append(items, false, false);
    
    /// <summary>
    /// Append an enumerable of pairs to the map
    /// </summary>
    /// <param name="items">Pairs to add</param>
    /// <returns>New map with pairs added</returns>
    [Pure]
    public Map<K, V> Append(params (K, V)[] items)
        => Append((IEnumerable<(K Key, V Value)>) items);

    /// <summary>
    /// Append an enumerable of pairs to the map
    /// </summary>
    /// <param name="items">Pairs to add</param>
    /// <returns>New map with pairs added</returns>
    [Pure]
    public Map<K, V> Append(IEnumerable<KeyValuePair<K, V>> items)
        => Append(items.Map(ToValueTuple));

    /// <summary>
    /// Append an enumerable of pairs to the map
    /// </summary>
    /// <param name="items">Pairs to add</param>
    /// <returns>New map with pairs added</returns>
    [Pure]
    public Map<K, V> Append(params KeyValuePair<K, V>[] items)
        => Append(items.Map(ToValueTuple));

    /// <summary>
    /// Append an enumerable of pairs to the map
    /// </summary>
    /// <param name="items">Pairs to add</param>
    /// <returns>New map with pairs added</returns>
    [Pure]
    public Map<K, V> Append(IEnumerable<Tuple<K, V>> items)
        => Append(items.Map(ToValueTuple));

    /// <summary>
    /// Append an enumerable of pairs to the map
    /// </summary>
    /// <param name="items">Pairs to add</param>
    /// <returns>New map with pairs added</returns>
    [Pure]
    public Map<K, V> Append(params Tuple<K, V>[] items)
        => Append(items.Map(ToValueTuple));
    
    /// <summary>
    /// Append a map to this
    /// </summary>
    /// <param name="map">Map to append</param>
    /// <returns>New map with <paramref name="map"/> appended</returns>
    [Pure]
    public Map<K, V> Append(Map<K, V> map) {
        if (Count == 0) return map;
        if (map.Count == 0) return this;

        return Append(map.AsEnumerable());
    }

    #endregion

    #region Remove
    
    public Map<K, V> Remove(K key) {
        if (key is null) throw new ArgumentNullException(nameof(key));

        return _root
            .Remove(KeyComparer, key)
            .Node
            .Pipe(
                Wrap
                    .Flip()
                    .Apply(Count - 1));
    }
    
    public Map<K, V> RemoveRange(IEnumerable<K> keys) {
        if (keys is null) throw new ArgumentNullException(nameof(keys));

        (Node Res, int Count) seed = (_root, Count);
        (Node Res, int Count) removed = keys.Aggregate(seed, (acc, key) => {
            (var res, int count) = acc;
            (Node node, bool mutated) = res.Remove(KeyComparer, key);

            return (node, mutated ? count - 1 : count);
        });

        return Wrap(removed.Res, removed.Count);
    }
    
    public Map<K, V> RemoveRange(params K[] items)
        => RemoveRange((IEnumerable<K>) items);

    #endregion
    
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