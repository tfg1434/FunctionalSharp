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
    //public static class ArbitraryMap {
    // private static Gen<Map<K, V>> Empty<K, V>() where K : notnull
    //     => Gen.Constant(Map<K, V>.Empty);
    //
    // private static Gen<Map<K, V>> NonEmpty<K, V>() where K : notnull
    //     => from key in Arb.Generate<K>()
    //        from val in Arb.Generate<V>()
    //        from map in GenMap<K, V>()
    //        select map.Add(key, val);
    //
    // private static Gen<Map<K, V>> GenMap<K, V>() where K : notnull
    //     => from isEmpty in Arb.Generate<bool>()
    //        from map in isEmpty ? Empty<K, V>() : NonEmpty<K, V>()
    //        select map;
    //
    // public static Arbitrary<Map<K, V>> IDictionary<K, V>() where K : notnull
    //     => GenMap<K, V>().ToArbitrary();
    
    public static class ArbitraryMap {
        private static Map<K, V> MapFromLists<K, V>(K[] keys, V[] vals) where K : notnull {
            var map = Map<K, V>.Empty;
        
            for (int i = 0; i < keys.Length; i++)
                map = map.Add(keys[i], vals[i]);
        
            return map;
        }
        
        private static Gen<Map<K, V>> GenMap<K, V>() where K : notnull
            => from length in Arb.Generate<int>()
               from keys in Gen.ArrayOf(length, Arb.Generate<K>())
               from vals in Gen.ArrayOf(length, Arb.Generate<V>())
               select MapFromLists(keys, vals);
        
        public static Arbitrary<Map<K, V>> MapArb<K, V>() where K : notnull
            => GenMap<K, V>().ToArbitrary();
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
        public void Remove_IntBool_EqualsMutable(SortedDictionary<int, bool> expected, int index) {
            Map<int, bool> actual = expected.ToMap();
            int key = expected.Skip(index).First().Key;
            actual = actual.Remove(key);
            expected.Remove(key);
            
            Assert.Equal(expected.ToList(), actual.ToList<KeyValuePair<int, bool>>());
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryMap) })]
        public void Add_ExistingKeySameValue_Same(Map<int, bool> map, int key, bool val) {
            map = map.Add(key, val);
            
            Assert.Equal(map, map.Add(key, val));
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryMap) })]
        public void AddRange_ExistingKeySameValue_Same(Map<int, bool> map, int key, bool val) {
            map = map.Add(key, val);
        
            Assert.Equal(map, map.AddRange(new[] { (key, val) }));
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryMap) })]
        public void Add_ExisitingKeyDiffValue_Throws(Map<int, bool> map, int key, bool val1, bool val2) {
            if (map.ValComparer.Equals(val1, val2))
                Succeed();
        
            map = map.Add(key, val1);
        
            Assert.Throws<ArgumentException>(() => map.Add(key, val2));
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryMap) })]
        public void AddRange_ExistingKeyDiffValue_Throws(Map<int, bool> map, int key, bool val1, bool val2) {
            if (map.ValueComparer.Equals(val1, val2))
                Succeed();
        
            map = map.Add(key, val1);
        
            Assert.Throws<ArgumentException>(
                () => map.AddRange(new[] { (key, val2) }));
        }
        
        [Fact]
        public void SetItems_NullKey_Throws() {
            (string, bool)[] items = { (null!, default) };
            
            Assert.Throws<ArgumentNullException>(() => Map<string, int>().SetItems(items));
        }
        
        [Property]
        public void ContainsValue(int key, bool val) {
            var map = Map<int, bool>();
            Assert.False(map.ContainsValue(val));
            Assert.True(map.Add(key, val).ContainsValue(val));
        }
        
        [Fact]
        public void Create() {
            IComparer<string> keyComparer = StringComparer.OrdinalIgnoreCase;
            IEqualityComparer<string> valComparer = StringComparer.CurrentCulture;
            
            var map = Map<string, string>();
            Assert.Empty(map);
            Assert.Same(Comparer<string>.Default, map.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, map.ValComparer);

            map = Map<string, string>(keyComparer);
            Assert.Empty(map);
            Assert.Same(keyComparer, map.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, map.ValComparer);

            map = Map<string, string>(valComparer);
            Assert.Empty(map);
            Assert.Same(Comparer<string>.Default, map.KeyComparer);
            Assert.Same(valComparer, map.ValComparer);
            
            map = Map<string, string>(keyComparer, valComparer);
            Assert.Empty(map);
            Assert.Same(keyComparer, map.KeyComparer);
            Assert.Same(valComparer, map.ValComparer);

            (string, string)[] pairs = { ("a", "b") };
            
            map = Map<string, string>(pairs);
            Assert.Single(map);
            Assert.Same(Comparer<string>.Default, map.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, map.ValComparer);
            
            map = Map<string, string>(keyComparer, pairs);
            Assert.Single(map);
            Assert.Same(keyComparer, map.KeyComparer);
            Assert.Same(EqualityComparer<string>.Default, map.ValComparer);
            
            map = Map<string, string>(valComparer, pairs);
            Assert.Single(map);
            Assert.Same(Comparer<string>.Default, map.KeyComparer);
            Assert.Same(valComparer, map.ValComparer);
            
            map = Map<string, string>(keyComparer, valComparer, pairs);
            Assert.Single(map);
            Assert.Same(keyComparer, map.KeyComparer);
            Assert.Same(valComparer, map.ValComparer);
        }
    }
}