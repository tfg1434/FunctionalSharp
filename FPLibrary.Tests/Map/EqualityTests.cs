using System;
using FPLibrary;
using FsCheck.Xunit;
using Xunit;
using static FPLibrary.Tests.Utils;
using static FPLibrary.F;

namespace FPLibrary.Tests.Map {
    public class MapEqualityTests {
        [Property(Arbitrary = new[] { typeof(ArbitraryMap) })]
        public void Equal_HashCode_Holds(Map<int, bool> map1, Map<int, bool> map2) {
            if (map1 == map2)
                Assert.Equal(map1.GetHashCode(), map2.GetHashCode());
            else
                Assert.NotEqual(map1.GetHashCode(), map2.GetHashCode());
        }
        
        [Property(Arbitrary = new[] { typeof(ArbitraryMap) })]
        public void HashCode_NoThrow_Holds(Map<int, bool> map) {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            /*Assert.DoesNotThrow(*/map.GetHashCode();
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
    }
}