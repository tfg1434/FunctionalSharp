using System;
using FunctionalSharp;
using FsCheck.Xunit;
using Xunit;
using static FunctionalSharp.Tests.Utils;
using static FunctionalSharp.F;

namespace FunctionalSharp.Tests.Map {
    public class MapEqualityTests {
        [Fact]
        public void Equal_HashCode_Holds() {
            var map1 = Map<string, string>(("e", "f"), ("e", "f"));
            var map2 = Map<string, string>(("e", "f"), ("e", "f"));
            
            Assert.Equal(map1, map2);
            Assert.Equal(map1.GetHashCode(), map2.GetHashCode());

            map1 = MapWithComparers<string, string>(StringComparer.OrdinalIgnoreCase, null, ("e", "f"), ("e", "f"));
            map2 = Map<string, string>(("e", "f"), ("e", "f"));
            Assert.NotEqual(map1, map2);
            Assert.NotEqual(map1.GetHashCode(), map2.GetHashCode());
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryMap) })]
        public void HashCode_NoThrow_Holds(Map<int, bool> map) {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            /*Assert.DoesNotThrow(*/map.GetHashCode();
        }
        
        //https://stackoverflow.com/questions/7278136/create-hash-value-on-a-list
        [Fact(Skip = "dont think this applies for maps")]
        public void HashCode_Order_Matters() {
            int expected = Map<string, int>(("foo", 0), ("bar", 0), ("spam", 0)).GetHashCode();
            int actual = Map<string, int>(("spam", 0), ("bar", 0), ("foo", 0)).GetHashCode();
            
            Assert.NotEqual(expected, actual);
        }

        [Fact(Skip = "dont think this applies for maps")]
        public void HashCode_Duplicates_Matter() {
            int a = Map<string, int>(("foo", 0), ("bar", 0), ("spam", 0)).GetHashCode();
            int b = Map<string, int>(("foo", 0), ("bar", 0), ("spam", 0), ("foo", 0), ("foo", 0)).GetHashCode();
            int c = Map<string, int>(("foo", 0), ("bar", 0), ("spam", 0), ("foo", 0), ("foo", 0), ("spam", 0), ("foo", 0), ("spam", 0), ("foo", 0)).GetHashCode();
            
            Assert.False(a == b && b == c && a == c);
        }
        
        [Fact]
        public void Equals_Operator_Equals() {
            var lhs = Map<int, bool>((0, true), (4, false));
            var rhs = Map<int, bool>((0, true), (4, false));
            
            Assert.True(lhs == rhs);
        }
        
        [Fact]
        public void Equals_Method_Equals() {
            var lhs = Map<int, bool>((0, true), (4, false));
            var rhs = Map<int, bool>((0, true), (4, false));
            
            Assert.True(lhs.Equals(rhs));
        }
        
        [Fact]
        public void Equals_Operator_NotEqual() {
            var lhs = Map<int, bool>((0, true), (4, false));
            var rhs = Map<int, bool>((1, true), (4, false));
            
            Assert.False(lhs == rhs);
        }
        
        [Fact]
        public void Equals_Method_NotEqual() {
            var lhs = Map<int, bool>((0, true), (4, false));
            var rhs = Map<int, bool>((1, true), (4, false));
            
            Assert.False(lhs.Equals(rhs));
        }

        [Fact]
        public void Equals_Comparer_Uses() {
            //TODO: Implement
        }
    }
}