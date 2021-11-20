using System;
using Xunit;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Xunit;
using System.Collections.Generic;
using static FunctionalSharp.F;
using static FunctionalSharp.Tests.Utils;

namespace FunctionalSharp.Tests.IEnumerable {
    public class MonadLawTests {
        //sorta a monad, but no implementation of Return

        //(m >>= f) >>= g == m >>= (x => f(x) >>= g)
        [Property(Arbitrary = new[] { typeof(ArbitraryIEnumerable) })]
        public void AssociativeHolds(IEnumerable<int> m) {
            Func<int, IEnumerable<int>> f = x => new[] { Times2(x) };
            Func<int, IEnumerable<int>> g = x => new[] {Plus5(x)};

            IEnumerable<int> expected = m.Bind(f).Bind(g);
            IEnumerable<int> actual = m.Bind(x => f(x).Bind(g));

            Assert.Equal(expected, actual);
        }
    }
}
