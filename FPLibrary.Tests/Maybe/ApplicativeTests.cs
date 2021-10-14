using System;
using Xunit;
using FsCheck.Xunit;
using FsCheck;
using FPLibrary;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.Maybe {
    public class ApplicativeTests {
        [Property]
        public void Apply_Nothing_Nothing(int i) {
            Maybe<int> m = Just(i)
                .Map(Add)
                .Apply(Nothing);

            Assert.True(m.IsNothing);
        }

        [Fact]
        public void Map_ToNothing_Nothing() {
            Maybe<int> m = ((Maybe<int>) Nothing)
                .Map(Add)
                .Apply(0);

            Assert.True(m.IsNothing);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        public void Apply_TwoArgs(int a, int b, int result) {
            Maybe<int> expected = Just(result);
            Maybe<int> actual = Just(Add)
                .Apply(Just(a))
                .Apply(Just(b));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 6)]
        public void Apply_ThreeArgs(int a, int b, int c, int result) {
            Maybe<int> expected = Just(result);
            Maybe<int> actual = Just(Add3)
                .Apply(Just(a))
                .Apply(Just(b))
                .Apply(Just(c));
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, 10)] 
        public void Apply_FourArgs(int a, int b, int c, int d, int result) {
            Maybe<int> expected = Just(result);
            Maybe<int> actual = Just(Add4)
                .Apply(Just(a))
                .Apply(Just(b))
                .Apply(Just(c))
                .Apply(Just(d));
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, 5, 15)]
        public void Apply_FiveArgs(int a, int b, int c, int d, int e, int result) {
            Maybe<int> expected = Just(result);
            Maybe<int> actual = Just(Add5)
                .Apply(Just(a))
                .Apply(Just(b))
                .Apply(Just(c))
                .Apply(Just(d))
                .Apply(Just(e));
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, 5, 6, 21)]
        public void Apply_SixArgs(int a, int b, int c, int d, int e, int f, int result) {
            Maybe<int> expected = Just(result);
            Maybe<int> actual = Just(Add6)
                .Apply(Just(a))
                .Apply(Just(b))
                .Apply(Just(c))
                .Apply(Just(d))
                .Apply(Just(e))
                .Apply(Just(f));
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2, 3, 4, 5, 6, 7, 28)]
        public void Apply_SevenArgs(int a, int b, int c, int d, int e, int f, int g, int result) {
            Maybe<int> expected = Just(result);
            Maybe<int> actual = Just(Add7)
                .Apply(Just(a))
                .Apply(Just(b))
                .Apply(Just(c))
                .Apply(Just(d))
                .Apply(Just(e))
                .Apply(Just(f))
                .Apply(Just(g));

            Assert.Equal(expected, actual);
        }
    }
}
