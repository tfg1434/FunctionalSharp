using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLibrary {
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

        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer=null,
            IEqualityComparer<V>? valComparer=null, params KeyValuePair<K, V>[] items) where K : notnull

            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer=null,
            IEqualityComparer<V>? valComparer=null, params Tuple<K, V>[] items) where K : notnull

            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IEnumerable<KeyValuePair<K, V>> items,
            IComparer<K>? keyComparer=null, IEqualityComparer<V>? valComparer=null) where K : notnull

            => MapWithComparers(keyComparer, valComparer).AddRange(items);
        
        public static Map<K, V> MapWithComparers<K, V>(IEnumerable<Tuple<K, V>> items,
            IComparer<K>? keyComparer=null, IEqualityComparer<V>? valComparer=null) where K : notnull

            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        #endregion

        #region ToMap



        #endregion
    }

    public sealed partial class Map<K, V> where K : notnull {
        public Map<K, V> Add(K key, V val) => Add((key, val));
        public Map<K, V> Add(KeyValuePair<K, V> kv) => Add(ToValueTuple(kv));
        public Map<K, V> Add(Tuple<K, V> tup) => Add(ToValueTuple(tup));

        public Map<K, V> AddRange(IEnumerable<KeyValuePair<K, V>> items) => AddRange(items.Map(ToValueTuple));
        public Map<K, V> AddRange(IEnumerable<Tuple<K, V>> items) 
            => AddRange(items.Map(ToValueTuple));

        public Map<K, V> SetItem(K key, V val) => SetItem((key, val));
        public Map<K, V> SetItem(KeyValuePair<K, V> kv) => SetItem(ToValueTuple(kv));
        public Map<K, V> SetItem(Tuple<K, V> tup) => SetItem(ToValueTuple(tup));
    }
}