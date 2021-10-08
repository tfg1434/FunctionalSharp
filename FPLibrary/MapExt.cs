using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLibrary {
    public static partial class F {
        public static Map<K, V> Map<K, V>() where K : notnull
            => FPLibrary.Map<K, V>.Empty;
        
        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer=null, 
            IEqualityComparer<V>? valComparer=null) where K : notnull

            => FPLibrary.Map<K, V>.Empty.WithComparers(keyComparer, valComparer);

        public static Map<K, V> Map<K, V>(params (K Key, V Val)[] items) where K : notnull
            => Map<K, V>().AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IComparer<K>? keyComparer=null, 
            IEqualityComparer<V>? valComparer=null, params (K Key, V Val)[] items) where K : notnull

            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        public static Map<K, V> Map<K, V>(IEnumerable<(K Key, V Val)> items) where K : notnull
            => Map<K, V>().AddRange(items);

        public static Map<K, V> MapWithComparers<K, V>(IEnumerable<(K Key, V Val)> items, 
            IComparer<K>? keyComparer=null, IEqualityComparer<V>? valComparer=null) where K : notnull

            => MapWithComparers(keyComparer, valComparer).AddRange(items);

        //to Map with no projections and default comparers
        public static Map<K, V> ToMap<K, V>(this IEnumerable<(K Key, V Val)> src) where K : notnull
            => Map(src.ToArray());
        //TODO: use params IEnumerable

        //to Map with default comparers and no projections
        public static Map<K, V> ToMap<K, V>(this IEnumerable<KeyValuePair<K, V>> src) where K : notnull
            => src.ToMap(item => item.Key, item => item.Value);

        //to Map with both key and value projections
        public static Map<K, V> ToMap<T, K, V>(this IEnumerable<T> src, Func<T, K> keyProj, Func<T, V> valProj)
            where K : notnull

            => src.ToMapWithComparers(keyProj, valProj);

        //to Map with key and value comparers
        public static Map<K, V> ToMapWithComparers<K, V>(this IEnumerable<(K Key, V Val)> src, 
            IComparer<K>? keyComparer=null, IEqualityComparer<V>? valComparer=null) where K : notnull {
            
            if (src is null) throw new ArgumentNullException(nameof(src));

            keyComparer ??= Comparer<K>.Default;
            valComparer ??= EqualityComparer<V>.Default;

            return MapWithComparers(keyComparer, valComparer)
                .AddRange(src);
        }
        
        //to Map with both key and value projections & key and value comparers
        public static Map<K, V> ToMapWithComparers<T, K, V>(this IEnumerable<T> src, Func<T, K> keyProj, 
            Func<T, V> valProj, IComparer<K>? keyComparer=null, IEqualityComparer<V>? valComparer=null) 
            where K : notnull {
            
            if (keyProj is null) throw new ArgumentNullException(nameof(keyProj));
            if (valProj is null) throw new ArgumentNullException(nameof(valProj));

            return src
                .Map(t => (keyProj(t), valProj(t)))
                .ToMapWithComparers(keyComparer, valComparer);
        }
    }
    
    public sealed partial class Map<K, V> where K : notnull {
        public Map<K, V> SetItem(K key, V val) => SetItem((key, val));
        
        public Map<K, V> Add(K key, V val) => Add((key, val));
    }
}