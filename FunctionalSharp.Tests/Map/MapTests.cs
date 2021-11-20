using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using FunctionalSharp;
using static FunctionalSharp.F;
using static FunctionalSharp.Tests.Utils;
#pragma warning disable CS8619

namespace FunctionalSharp.Tests.Map {
    public static class ArbitraryMap {
        private static Gen<Map<K, V>> Empty<K, V>() where K : notnull
            => Gen.Constant(F.Map<K, V>());
        
        private static Gen<Map<K, V>> NonEmpty<K, V>() where K : notnull
            => from key in Arb.Generate<K>()
               from val in Arb.Generate<V>()
               from map in GenMap<K, V>()
               select map.ContainsKey(key) ? map : map.Add(key, val);
        
        private static Gen<Map<K, V>> GenMap<K, V>() where K : notnull
            => from isEmpty in Arb.Generate<bool>()
               from map in isEmpty ? Empty<K, V>() : NonEmpty<K, V>()
               select map;
        
        public static Arbitrary<Map<K, V>> Map<K, V>() where K : notnull
            => GenMap<K, V>().ToArbitrary();
    }

    public static class ArbitraryImmutableSortedDictionary {
        private static Gen<ImmutableSortedDictionary<K, V>> Empty<K, V>() where K : notnull
            => Gen.Constant(System.Collections.Immutable.ImmutableSortedDictionary<K, V>.Empty);
        
        private static Gen<ImmutableSortedDictionary<K, V>> NonEmpty<K, V>() where K : notnull
            => from key in Arb.Generate<K>()
               from val in Arb.Generate<V>()
               from map in GenImmutableSortedDictionary<K, V>()
               select map.ContainsKey(key) ? map : map.Add(key, val);
        
        private static Gen<ImmutableSortedDictionary<K, V>> GenImmutableSortedDictionary<K, V>() where K : notnull
            => from isEmpty in Arb.Generate<bool>()
               from map in isEmpty ? Empty<K, V>() : NonEmpty<K, V>()
               select map;
        
        public static Arbitrary<ImmutableSortedDictionary<K, V>> ImmutableSortedDictionary<K, V>() where K : notnull
            => GenImmutableSortedDictionary<K, V>().ToArbitrary();
    }
    
    public class MapTests {
        [Property(Arbitrary = new[] { typeof(ArbitraryImmutableSortedDictionary) })]
        public void Add_IntBool_EqualsBuiltin(ImmutableSortedDictionary<int, bool> expected) {
            var actual = Map<int, bool>.Empty;
        
            foreach ((int key, bool val) in expected)
                actual = actual.Add(key, val);
        
            Assert.Equal(expected.ToList(), actual.ToList<KeyValuePair<int, bool>>());
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryImmutableSortedDictionary) })]
        public void Set_IntBool_EqualsMutable(ImmutableSortedDictionary<int, bool> expected) {
            var actual = Map<int, bool>.Empty;
        
            foreach ((int key, bool val) in expected) {
                actual = actual.Add(key, default);
                actual = actual.SetItem(key, val);
            }
            
            Assert.Equal(expected.ToList(), actual.ToList<KeyValuePair<int, bool>>());
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryImmutableSortedDictionary) })]
        public void Remove_IntBool_EqualsMutable(ImmutableSortedDictionary<int, bool> expected) {
            if (expected.Count == 0) {
                Succeed();
                return;
            }

            int index = new Random().Next(0, expected.Count);
            
            Map<int, bool> actual = expected.ToMap();
            int key = expected.Skip(index).First().Key;
            actual = actual.Remove(key);
            expected = expected.Remove(key);
            
            Assert.Equal(expected.ToList(), actual.ToList<KeyValuePair<int, bool>>());
        }
        
        [Property]
        public void Add_ExistingKeySameValue_Same(int key, bool val) {
            var map = Map<int, bool>();
            map = map.Add(key, val);
            
            Assert.Equal(map, map.Add(key, val));
        }
        
        [Property]
        public void Add_ExisitingKeyDiffValue_Throws(int key, bool val) {
            var map = Map<int, bool>();
            map = map.Add(key, val);
            
            Assert.Throws<ArgumentException>(() => map.Add(key, !val));
        }

        [Fact]
        public void Add_NullKey_Throws() {
            Assert.Throws<ArgumentNullException>(
                () => Map<string, bool>().Add(null!, default));
        }
        
        [Property]
        public void AddRange_ExistingKeySameValue_Same(int key, bool val) {
            var map = Map<int, bool>();
            map = map.Add(key, val);
        
            Assert.Equal(map, map.AddRange((key, val)));
        }

        [Property]
        public void AddRange_ExistingKeyDiffValue_Throws(int key, bool val) {
            var map = Map<int, bool>();
            map = map.Add(key, val);
        
            Assert.Throws<ArgumentException>(() => map.AddRange((key, !val)));
        }
        
        [Fact]
        public void AddRange_NullKey_Throws() {
            Assert.Throws<ArgumentNullException>(
                () => Map<string, bool>().AddRange((null!, default)));
        }

        [Fact]
        [SuppressMessage("ReSharper", "xUnit2004")]
        public void AddRange_Duplicates_Ignores() {
            var map = Map<int, bool>((0, true), (0, true));

            Assert.Single(map);
            Assert.Equal(true, map[0]);
        }

        [Property(Skip = "Keys not implemented")]
        public void SetItem_ExistingKey_Updates(int key, bool val1, bool val2) {
            var map = Map<int, bool>((key, val1));
            map = map.SetItem(key, val2);
            
            Assert.Equal(key, map.Keys.Single());
        }

        [Fact]
        public void SetItems_NullKey_Throws() {
            Assert.Throws<ArgumentNullException>(
                () => Map<string, bool>().SetItems((null!, default)));
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryImmutableSortedDictionary) })]
        public void ContainsKey_IntBool_EqualsMutable(ImmutableSortedDictionary<int, bool> dict, int key) {
            var map = dict.ToMap();
            
            Assert.Equal(dict.ContainsKey(key), map.ContainsKey(key));
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryImmutableSortedDictionary) })]
        public void ContainsValue_IntBool_EqualsMutable(ImmutableSortedDictionary<int, bool> dict, bool val) {
            var map = dict.ToMap();
            
            Assert.Equal(dict.ContainsValue(val), map.ContainsVal(val));
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
        
        [Fact]
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
            // ReSharper disable once ArrangeMethodOrOperatorBody
            Assert.Equal(Map<int, bool>.Empty, map.Clear());
        }
        
        [Fact]
        public void Clear_WithComparer_KeepsComparer() {
            var map = MapWithComparers<string, string>(StringComparer.OrdinalIgnoreCase, null, ("a", "0"));
            
            Assert.NotEqual(Map<string, string>.Empty, map.Clear());
            Assert.Equal(MapWithComparers<string, string>(StringComparer.OrdinalIgnoreCase), map.Clear());
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryImmutableSortedDictionary) })]
        public void Get_IntBool_EqualsBuiltin(ImmutableSortedDictionary<int, bool> dict, int key) {
            var map = dict.ToMap();

            Assert.Equal(dict.ContainsKey(key), map.Get(key).IsJust);
        }
        
        [Fact]
        public void RemoveRange_IntBool_Works() {
            var actual = Map<int, bool>((5, true), (3, false)).RemoveRange(new[] { 5 });
            var expected = Map<int, bool>((3, false));
            Assert.Equal(expected, actual);

            actual = Map<int, bool>((-1, false), (6, true), (4, true)).RemoveRange(4, 6);
            expected = Map<int, bool>((-1, false));
            Assert.Equal(expected, actual);
        }
    }
}