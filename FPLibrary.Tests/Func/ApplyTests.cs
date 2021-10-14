using System;
using static FPLibrary.Tests.Utils;
using Xunit;

namespace FPLibrary.Tests.Func {
    public class ApplyTests {
        [Theory]
        [InlineData(1, 2, 3)]
        public void Apply_TwoArgs(int a, int b, int result) {
            int expected = result;
            int actual = Add
                .Apply(a)(b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 6)]
        public void Apply_ThreeArgs(int a, int b, int c, int result) {
            int expected = result;
            int actual = Add3
                .Apply(a)
                .Apply(b)(c);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, 10)]
        public void Apply_FourArgs(int a, int b, int c, int d, int result) {
            int expected = result;
            int actual = Add4
                .Apply(a)
                .Apply(b)
                .Apply(c)(d);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, 5, 15)]
        public void Apply_FiveArgs(int a, int b, int c, int d, int e, int result) {
            int expected = result;
            int actual = Add5
                .Apply(a)
                .Apply(b)
                .Apply(c)
                .Apply(d)(e);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, 5, 6, 21)]
        public void Apply_SixArgs(int a, int b, int c, int d, int e, int f, int result) {
            int expected = result;
            int actual = Add6
                .Apply(a)
                .Apply(b)
                .Apply(c)
                .Apply(d)
                .Apply(e)(f);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, 5, 6, 7, 28)]
        public void Apply_SevenArgs(int a, int b, int c, int d, int e, int f, int g, int result) {
            int expected = result;
            int actual = Add7
                .Apply(a)
                .Apply(b)
                .Apply(c)
                .Apply(d)
                .Apply(e)
                .Apply(f)(g);

            Assert.Equal(expected, actual);
        }
    }
}
