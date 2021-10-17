using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests {
    public class RangeTests {
        [Fact]
        public void Range_From_Infinite() {
            IEnumerable<int> expected = new[] { 1, 2, 3, 4, 5 };
            IEnumerable<int> actual = Range("1..").Take(5);
            
            Assert.Equal(expected, actual);

            expected = new[] { -5, -4, -3 };
            actual = Range("-5..").Take(3);
            
            Assert.Equal(expected, actual);
        }
    }
}