using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using FPLibrary;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.Map {
    // public static class ArbitraryMap {
    //     private static Gen<Map<K, V>> Empty<K, V>() where K : notnull
    //         => Gen.Constant(Map<K, V>.Empty);
    //
    //     private static Gen<Map<K, V>> NonEmpty<K, V>() where K : notnull
    //         => from key in Arb.Generate<K>()
    //            from val in Arb.Generate<V>()
    //            from map in GenMap<K, V>()
    //            select map.Add(key, val);
    //
    //     private static Gen<Map<K, V>> GenMap<K, V>() where K : notnull
    //         => from isEmpty in Arb.Generate<bool>()
    //            from map in isEmpty ? Empty<K, V>() : NonEmpty<K, V>()
    //            select map;
    //
    //     public static Arbitrary<Map<K, V>> IDictionary<K, V>() where K : notnull
    //         => GenMap<K, V>().ToArbitrary();
    // }
    
    // public static class ArbitraryMap {
    //     private static Map<K, V> MapFromLists<K, V>(K[] keys, V[] vals) where K : notnull {
    //         //too lazy to just not generate duplicates
    //         keys = keys.Distinct().ToArray();
    //         vals = vals.Distinct().ToArray();
    //         
    //         var map = Map<K, V>.Empty;
    //     
    //         for (int i = 0; i < keys.Length; i++)
    //             map = map.Add(keys[i], vals[i]);
    //     
    //         return map;
    //     }
    //     
    //     private static Gen<Map<K, V>> GenMap<K, V>() where K : notnull
    //         => from length in Arb.Generate<int>()
    //            from keys in Gen.ArrayOf(length, Arb.Generate<K>())
    //            from vals in Gen.ArrayOf(length, Arb.Generate<V>())
    //            select MapFromLists(keys, vals);
    //     
    //     public static Arbitrary<Map<K, V>> MapArb<K, V>() where K : notnull
    //         => GenMap<K, V>().ToArbitrary();
    // }
    
    //TODO: Generate tuples instead of two arrays
    public static class ArbitraryMap {
        private static Gen<Map<K, V>> GenMap<K, V>() where K : notnull
            => from length in Arb.Generate<int>()
               from lowerBound in Arb.Generate<int>()
               from upperBound in Arb.Generate<int>()
               from arr in Gen.ArrayOf(length, Gen.Two(Gen.Choose(lowerBound, upperBound)))
               select arr.ToMap();
    }
    
    public static class ArbitrarySortedDictionary {
        private static SortedDictionary<K, V> SortedDictFromLists<K, V>(K[] keys, V[] vals) where K : notnull {
            SortedDictionary<K, V> dict = new();
    
            for (int i = 0; i < keys.Length; i++)
                dict[keys[i]] = vals[i];
    
            return dict;
        }
    
        private static Gen<SortedDictionary<K, V>> GenSortedDictionary<K, V>() where K : notnull
            => from length in Arb.Generate<int>()
               from keys in Gen.ArrayOf(length, Arb.Generate<K>())
               from vals in Gen.ArrayOf(length, Arb.Generate<V>())
               select SortedDictFromLists(keys, vals);
    
        public static Arbitrary<SortedDictionary<K, V>> SortedDictionary<K, V>() where K : notnull
            => GenSortedDictionary<K, V>().ToArbitrary();
    }
    
    public class MapTests {
        [Property(Arbitrary = new[] { typeof(ArbitrarySortedDictionary) })]
        public void Add_IntBool_EqualsMutable(SortedDictionary<int, bool> expected) {
            var actual = Map<int, bool>.Empty;
        
            foreach ((int key, bool val) in expected)
                actual = actual.Add(key, val);
        
            Assert.Equal(expected.ToList(), actual.ToList<KeyValuePair<int, bool>>());
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitrarySortedDictionary) })]
        public void Set_IntBool_EqualsMutable(SortedDictionary<int, bool> expected) {
            var actual = Map<int, bool>.Empty;
        
            foreach ((int key, bool val) in expected) {
                actual = actual.Add(key, default);
                actual = actual.SetItem(key, val);
            }
            
            Assert.Equal(expected.ToList(), actual.ToList<KeyValuePair<int, bool>>());
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitrarySortedDictionary) })]
        public void Remove_IntBool_EqualsMutable(SortedDictionary<int, bool> expected) {
            if (expected.Count == 0) {
                Succeed();
                return;
            }

            int index = new Random().Next(0, expected.Count);
            
            Map<int, bool> actual = expected.ToMap();
            int key = expected.Skip(index).First().Key;
            actual = actual.Remove(key);
            expected.Remove(key);
            
            Assert.Equal(expected.ToList(), actual.ToList<KeyValuePair<int, bool>>());
        }
        
        [Property]
        public void Add_ExistingKeySameValue_Same(int key, bool val) {
            var map = Map<int, bool>();
            map = map.Add(key, val);
            
            Assert.Equal(map, map.Add(key, val));
        }
        
        [Property]
        public void AddRange_ExistingKeySameValue_Same(int key, bool val) {
            var map = Map<int, bool>();
            map = map.Add(key, val);
        
            Assert.Equal(map, map.AddRange(new[] { (key, val) }));
        }
        
        [Property]
        public void Add_ExisitingKeyDiffValue_Throws(int key, bool val) {
            var map = Map<int, bool>();
            map = map.Add(key, val);
            
            Assert.Throws<ArgumentException>(() => map.Add(key, !val));
        }
        
        [Property]
        public void AddRange_ExistingKeyDiffValue_Throws(int key, bool val) {
            var map = Map<int, bool>();
            map = map.Add(key, val);
        
            Assert.Throws<ArgumentException>(() => map.AddRange(new[] { (key, !val) }));
        }

        [Property]
        public void SetItem_ExistingKey_Updates(int key, bool val1, bool val2) {
            var map = Map<int, bool>((key, val1));
            map = map.SetItem(key, val2);
            
            Assert.Equal(key, map.Keys.Single());
        }

        // [Fact]
        // public void SetItems_NullKey_Throws() {
        //     (string, bool)[] items = { (null!, default) };
        //     
        //     Assert.Throws<ArgumentNullException>(() => Map<string, int>().SetItems(items));
        // }
        
        [Property(Arbitrary = new[] { typeof(ArbitrarySortedDictionary) })]
        public void ContainsKey_IntBool_EqualsMutable(SortedDictionary<int, bool> mutable, int key) {
            var map = mutable.ToMap();
            
            Assert.Equal(mutable.ContainsKey(key), map.ContainsKey(key));
        }

        [Property(Arbitrary = new[] { typeof(ArbitrarySortedDictionary) })]
        public void ContainsValue_IntBool_EqualsMutable(SortedDictionary<int, bool> mutable, bool val) {
            var map = mutable.ToMap();
            
            Assert.Equal(mutable.ContainsValue(val), map.ContainsValue(val));
        }

        [Fact]
        public void Create() {
            IComparer<string> keyComparer = StringComparer.OrdinalIgnoreCase;
            IEqualityComparer<string> valComparer = StringComparer.CurrentCulture;
            
            var map = Map<string, string>();
            Assert.Empty(map);
            Assert.Same(Comparer<string>.Default, map.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, map.ValComparer);

            map = MapWithComparers<string, string>(keyComparer: keyComparer);
            Assert.Empty(map);
            Assert.Same(keyComparer, map.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, map.ValComparer);

            map = MapWithComparers<string, string>(valComparer: valComparer);
            Assert.Empty(map);
            Assert.Same(Comparer<string>.Default, map.KeyComparer);
            Assert.Same(valComparer, map.ValComparer);
            
            map = MapWithComparers(keyComparer, valComparer);
            Assert.Empty(map);
            Assert.Same(keyComparer, map.KeyComparer);
            Assert.Same(valComparer, map.ValComparer);

            (string, string)[] pairs = { ("a", "b") };
            
            map = Map<string, string>(pairs);
            Assert.Single(map);
            Assert.Same(Comparer<string>.Default, map.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, map.ValComparer);
            
            map = MapWithComparers(pairs, keyComparer: keyComparer);
            Assert.Single(map);
            Assert.Same(keyComparer, map.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, map.ValComparer);
            
            map = MapWithComparers(pairs, valComparer: valComparer);
            Assert.Single(map);
            Assert.Same(Comparer<string>.Default, map.KeyComparer);
            Assert.Same(valComparer, map.ValComparer);
            
            map = MapWithComparers(pairs, keyComparer, valComparer);
            Assert.Single(map);
            Assert.Same(keyComparer, map.KeyComparer);
            Assert.Same(valComparer, map.ValComparer);
        }
        
        [Fact(Skip = "Not implemented")]
        public void Map_Comparers_UsesThem() {
            var map = MapWithComparers<string, string>(keyComparer: StringComparer.OrdinalIgnoreCase, null, 
                ("a", "0"), ("A", "0"));
            
            Assert.Same(StringComparer.OrdinalIgnoreCase, map.KeyComparer);
            Assert.Single(map);
            Assert.True(map.ContainsKey("a"));
            Assert.Equal("0", map["a"]);
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryMap) })]
        public void Clear_NoComparer_NoComparer(Map<int, bool> map) {
            Assert.Same(Map<int, bool>.Empty, map.Clear());
        }
        
        [Fact]
        public void Clear_WithComparer_KeepsComparer() {
            var map = MapWithComparers<string, string>(StringComparer.OrdinalIgnoreCase, null, ("a", "0"));
            
            Assert.NotSame(Map<string, string>.Empty, map.Clear());
            Assert.Same(MapWithComparers<string, string>(StringComparer.OrdinalIgnoreCase), map);
        }
    }
}