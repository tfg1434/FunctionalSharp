using System;
using System.Collections;
using System.Collections.Generic;

namespace FunctionalSharp {
    public static partial class F {
        #region Map

        public static Map<K, V> Map<K, V>(params KeyValuePair<K, V>[] items) where K : notnull
            => Map((IEnumerable<KeyValuePair<K, V>>) items);

        public static Map<K, V> Map<K, V>(params Tuple<K, V>[] items) where K : notnull
            => Map((IEnumerable<Tuple<K, V>>) items);

        public static Map<K, V> Map<K, V>(IEnumerable<KeyValuePair<K, V>> items) where K : notnull
            => Map<K, V>().AddRange(items);

        public static Map<K, V> Map<K, V>(IEnumerable<Tuple<K, V>> items) where K : notnull
            => Map<K, V>().AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
            IEqualityComparer<V>? valComparer = null, params KeyValuePair<K, V>[] items) where K : notnull
            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer = null,
            IEqualityComparer<V>? valComparer = null, params Tuple<K, V>[] items) where K : notnull
            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IEnumerable<KeyValuePair<K, V>> items,
            IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull
            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IEnumerable<Tuple<K, V>> items,
            IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull
            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        #endregion

        #region ToMap

        //to Map with no projections and default comparers
        public static Map<K, V> ToMap<K, V>(this IEnumerable<KeyValuePair<K, V>> src) where K : notnull
            => Map(src);

        public static Map<K, V> ToMap<K, V>(this IEnumerable<Tuple<K, V>> src) where K : notnull
            => Map(src);

        //to Map with key and value comparers
        public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<KeyValuePair<K, V>> src,
            IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull {
            if (src is null) throw new ArgumentNullException(nameof(src));

            return MapWithComparers(keyComparer, valComparer)
                .AddRange(src);
        }

        public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<Tuple<K, V>> src,
            IComparer<K>? keyComparer = null, IEqualityComparer<V>? valComparer = null) where K : notnull {
            if (src is null) throw new ArgumentNullException(nameof(src));

            return MapWithComparers(keyComparer, valComparer)
                .AddRange(src);
        }

        #endregion
    }

    public sealed partial class Map<K, V> where K : notnull {
        public bool Contains(KeyValuePair<K, V> kv)
            => Contains(ToValueTuple(kv));

        public Map<K, V> Add(K key, V val) => Add((key, val));

        public Map<K, V> Add(KeyValuePair<K, V> kv) => Add(ToValueTuple(kv));

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
}