using System;
using Xunit;
using FsCheck.Xunit;
using FsCheck;
using FPLibrary;
using System.Collections.Generic;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.IEnumerable {
    public class FunctorLawTests {
        //map ident == ident
        [Property(Arbitrary = new[] { typeof(ArbitraryIEnumerable) })]
        public void IdentityHolds(IEnumerable<object?> m) {
            IEnumerable<object?> expected = m;
            IEnumerable<object?> actual = expected.Map(x => x);

            Assert.Equal(expected, actual);
        }

        //fmap (f . g) == fmap f . fmap g
        [Property(Arbitrary = new[] { typeof(ArbitraryIEnumerable) })]
        public void CompositionHolds(IEnumerable<int> m) {
            Func<int, int> f = Times2;
            Func<int, int> g = Plus5;

            IEnumerable<int> expected = m.Map(f).Map(g);
            IEnumerable<int> actual = m.Map(y => g(f(y)));

            Assert.Equal(expected, actual);
        }
    }
}
