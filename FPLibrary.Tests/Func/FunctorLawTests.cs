using System;
using System.Linq;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.Func {
    public class FunctorLawTests {
        //map ident == ident
        [Fact]
        public void IdentityHolds() {
            Func<int, int> expected = Times2;
            Func<int, int> actual = expected.Map(x => x);

            Assert.True(IsEqual(expected, actual));
        }

        //fmap (f . g) == fmap f . fmap g
        [Fact]
        public void CompositionHolds() {
            Func<int, int> f = Times2;
            Func<int, int> g = Plus5;

            Func<int, int> expected = Plus7.Map(f).Map(g);
            Func<int, int> actual = Plus7.Map(x => g(f(x)));

            Assert.True(IsEqual(expected, actual));
        }

        private static bool IsEqual(Func<int, int> fA, Func<int, int> fB)
            => Enumerable.Range(-10, 10)
                .All(x => fA(x) == fB(x));
    }
}