using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    /// See if the map contains a specific key and value
    /// </summary>
    /// <param name="key">Key to look for</param>
    /// <param name="value">Value to look for</param>
    /// <returns>Whether or not map contains this key and value</returns>
    [Pure]
    public bool Contains(K key, V value) 
        => _root.Contains(KeyComparer, ValueComparer, key, value);

    /// <summary>
    /// See if map contains a specific key and value
    /// </summary>
    /// <param name="pair">Pair to look for</param>
    /// <returns>Whether or not map contains this pair</returns>
    [Pure]
    public bool Contains((K Key, V Value) pair)
        => Contains(pair.Key, pair.Value);
    
    /// <summary>
    /// See if map contains a <see cref="KeyValuePair{TKey,TValue}"/>
    /// </summary>
    /// <param name="pair"><see cref="KeyValuePair{TKey,TValue}"/> to search for</param>
    /// <returns>Whether map contains this <see cref="KeyValuePair{TKey,TValue}"/></returns>
    [Pure]
    public bool Contains(KeyValuePair<K, V> pair)
        => Contains(pair.Key, pair.Value);
    
    /// <summary>
    /// See if map contains a specific key and value
    /// </summary>
    /// <param name="pair">Pair to look for</param>
    /// <returns>Whether or not map contains this pair</returns>
    [Pure]
    public bool Contains(Tuple<K, V> pair)
        => Contains(pair.Item1, pair.Item2);

    #endregion

    #region Append

    /// <summary>
    /// Append a <paramref name="key"/> and associated <paramref name="value"/> to the map
    /// </summary>
    /// <param name="key">Key to add</param>
    /// <param name="value">Value to add</param>
    /// <returns>New map with key and value added</returns>
    [Pure]
    public Map<K, V> Append(K key, V value) {
        (Node res, _) = _root.Add(KeyComparer, ValueComparer, key, value);

        return Wrap(res, Count + 1);
    }
    
    /// <summary>
    /// Append a pair to the map
    /// </summary>
    /// <param name="pair">Pair to add</param>
    /// <returns>New map with pair added</returns>
    [Pure]
    public Map<K, V> Append((K Key, V Value) pair) 
        => Append(pair.Key, pair.Value);

    /// <summary>
    /// Append a pair to the map
    /// </summary>
    /// <param name="pair">Pair to add</param>
    /// <returns>New map with pair added</returns>
    [Pure]
    public Map<K, V> Append(KeyValuePair<K, V> pair) 
        => Append(pair.Key, pair.Value);

    /// <summary>
    /// Append a pair to the map
    /// </summary>
    /// <param name="pair">Pair to add</param>
    /// <returns>New map with pair added</returns>
    [Pure]
    public Map<K, V> Append(Tuple<K, V> pair) 
        => Append(pair.Item1, pair.Item2);
    
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
    
    /// <summary>
    /// Remove a key from the map
    /// </summary>
    /// <param name="key">Key to remove</param>
    /// <returns>New map with key removed</returns>
    [Pure]
    public Map<K, V> Remove(K key) 
        => _root
            .Remove(KeyComparer, key)
            .Node
            .Pipe(
                Wrap
                    .Flip()
                    .Apply(Count - 1));

    /// <summary>
    /// Remove multiple keys from the map
    /// </summary>
    /// <param name="keys">Keys to remove</param>
    /// <returns>New map with keys removed</returns>
    [Pure]
    public Map<K, V> RemoveRange(IEnumerable<K> keys) {
        (Node Res, int Count) seed = (_root, Count);
        (Node Res, int Count) removed = keys.Aggregate(seed, (acc, key) => {
            (var res, int count) = acc;
            (Node node, bool mutated) = res.Remove(KeyComparer, key);

            return (node, mutated ? count - 1 : count);
        });

        return Wrap(removed.Res, removed.Count);
    }
    
    /// <summary>
    /// Remove multiple keys from the map
    /// </summary>
    /// <param name="keys">Keys to remove</param>
    /// <returns>New map with keys removed</returns>
    [Pure]
    public Map<K, V> RemoveRange(params K[] keys)
        => RemoveRange((IEnumerable<K>) keys);

    #endregion
    
    #region SetItem

    /// <summary>
    /// Set an item in the map
    /// </summary>
    /// <param name="key">Key to set</param>
    /// <param name="value">Value to set to</param>
    /// <returns>New map with updated value</returns>
    [Pure]
    public Map<K, V> SetItem(K key, V value) {
        (Node newRoot, bool replaced, _) = _root.Set(KeyComparer, ValueComparer, key, value);
        
        return Wrap(newRoot, replaced ? Count : Count + 1);
    }

    /// <summary>
    /// Set an item in the map
    /// </summary>
    /// <param name="pair">Pair to set</param>
    /// <returns>New map with updated value</returns>
    [Pure]
    public Map<K, V> SetItem((K Key, V Value) pair)
        => SetItem(pair.Key, pair.Value);
    
    /// <summary>
    /// Set an item in the map
    /// </summary>
    /// <param name="pair">Pair to set</param>
    /// <returns>New map with updated value</returns>
    [Pure]
    public Map<K, V> SetItem(KeyValuePair<K, V> pair) 
        => SetItem(pair.Key, pair.Value);

    /// <summary>
    /// Set an item in the map
    /// </summary>
    /// <param name="pair">Pair to set</param>
    /// <returns>New map with updated value</returns>
    [Pure]
    public Map<K, V> SetItem(Tuple<K, V> pair) 
        => SetItem(pair.Item1, pair.Item2);

    /// <summary>
    /// Set multiple items in the map
    /// </summary>
    /// <param name="items">Items to set</param>
    /// <returns>New, updated map</returns>
    [Pure]
    public Map<K, V> SetItems(params (K Key, V Value)[] items)
        => SetItems((IEnumerable<(K Key, V Value)>) items);

    /// <summary>
    /// Set multiple items in the map
    /// </summary>
    /// <param name="items">Items to set</param>
    /// <returns>New, updated map</returns>
    [Pure]
    public Map<K, V> SetItems(params KeyValuePair<K, V>[] items)
        => SetItems(items.Map(ToValueTuple));
    
    /// <summary>
    /// Set multiple items in the map
    /// </summary>
    /// <param name="items">Items to set</param>
    /// <returns>New, updated map</returns>
    [Pure]
    public Map<K, V> SetItems(params Tuple<K, V>[] items)
        => SetItems(items.Map(ToValueTuple));
    
    /// <summary>
    /// Set multiple items in the map
    /// </summary>
    /// <param name="items">Items to set</param>
    /// <returns>New, updated map</returns>
    [Pure]
    public Map<K, V> SetItems(IEnumerable<(K Key, V Value)> items)
        => Append(items, true, false);

    /// <summary>
    /// Set multiple items in the map
    /// </summary>
    /// <param name="items">Items to set</param>
    /// <returns>New, updated map</returns>
    [Pure]
    public Map<K, V> SetItems(IEnumerable<KeyValuePair<K, V>> items)
        => SetItems(items.Map(ToValueTuple));

    /// <summary>
    /// Set multiple items in the map
    /// </summary>
    /// <param name="items">Items to set</param>
    /// <returns>New, updated map</returns>
    [Pure]
    public Map<K, V> SetItems(IEnumerable<Tuple<K, V>> items)
        => SetItems(items.Map(ToValueTuple));
    
    #endregion
    
    /// <summary>
    /// Clear the map, but keep the comparers
    /// </summary>
    /// <returns>Empty map with comparers</returns>
    [Pure]
    public Map<K, V> Clear()
        => _root.IsEmpty
            ? this
            : Empty.WithComparers(KeyComparer, ValueComparer);

    /// <summary>
    /// Lookup the value for a key
    /// </summary>
    /// <param name="key">Key to lookup</param>
    /// <returns>Value associated with key</returns>
    [Pure]
    public Maybe<V> Get(K key) 
        => _root.Get(KeyComparer, key);
    
    /// <summary>
    /// Equivalent to Dictionary.TryGetValue.
    /// </summary>
    /// <param name="key">Key to try and get value from</param>
    /// <param name="value">Returned value</param>
    /// <returns>Whether value was found</returns>
    [Pure]
    public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value) 
        => _root.TryGetValue(KeyComparer, key, out value);

    /// <summary>
    /// Whether this map contains a particular key
    /// </summary>
    /// <param name="equalKey">Key to search for</param>
    /// <param name="actualKey">The matching key located in the dictionary if found, or <paramref name="equalKey"/>
    /// if no match is found.</param>
    /// <returns>Whether <paramref name="equalKey"/> was found</returns>
    [Pure]
    public bool TryGetKey(K equalKey, out K actualKey) 
        => _root.TryGetKey(KeyComparer, equalKey, out actualKey);
}