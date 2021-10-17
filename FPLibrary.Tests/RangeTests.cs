using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests {
    public class RangeTests {
        [Fact]
        public void Range_From_Int() {
            IEnumerable<int> expected = new[] { 1, 2, 3, 4, 5 };
            IEnumerable<int> actual = Range("1..").Take(5);
            
            Assert.Equal(expected, actual);

            expected = new[] { -5, -4, -3 };
            actual = Range("-5..").Take(3);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Range_FromTo_Int() {
            var expected = Enumerable.Empty<int>();
            IEnumerable<int> actual = Range("5..1");
            
            Assert.Equal(expected, actual);

            expected = new[] { -5 };
            actual = Range("-5..-5");
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Range_FromSecond_Int() {
            IEnumerable<int> expected = new[] { 5, 4, 3, 2, 1 };
            IEnumerable<int> actual = Range("5,4..").Take(5);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Range_FromSecondTo_Int() {
            IEnumerable<int> expected = new[] { 2, 7, 12 };
            IEnumerable<int> actual = Range("2,7..15");
            
            Assert.Equal(expected, actual);
        }
    }
}