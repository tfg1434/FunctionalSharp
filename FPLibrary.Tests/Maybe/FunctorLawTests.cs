using System;
using Xunit;
using FsCheck.Xunit;
using FsCheck;
using FPLibrary;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.Maybe {
    public class FunctorLawTests {
        //map ident == ident
        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void IdentityHolds(Maybe<object> m) {
            Maybe<object> expected = m;
            Maybe<object> actual = m.Map(x => x);

            Assert.Equal(expected, actual);
        }

        //fmap (f . g) == fmap f . fmap g
        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void CompositionHolds(Maybe<int> m) {
            Maybe<int> expected = m.Map(Times2).Map(Plus5);
            Maybe<int> actual = m.Map(x => Plus5(Times2(x)));

            Assert.Equal(expected, actual);
        }
    }
}
