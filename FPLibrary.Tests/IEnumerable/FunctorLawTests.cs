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
        [Property]
        public void IdentityHolds(object? o) {
            IEnumerable<object?> expected = new[] { o };
            IEnumerable<object?> actual = expected.Map(x => x);

            Assert.Equal(expected, actual);
        }

        //fmap (f . g) == fmap f . fmap g
        [Property]
        public void CompositionHolds(int x) {
            Func<int, int> f = Times2;
            Func<int, int> g = Plus5;

            IEnumerable<int> expected = new[] { x }.Map(f).Map(g);
            IEnumerable<int> actual = new[] { x }.Map(y => g(f(y)));

            Assert.Equal(expected, actual);
        }
    }
}
