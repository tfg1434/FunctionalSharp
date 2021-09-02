using System;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using static FPLibrary.F;

namespace FPLibrary.Tests {
    public class MaybeMonadLawTests {
        //m == m >>= Return
        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void RightIdentityHolds(Maybe<object> m) {
            Maybe<object> actual = m.Bind(Just);
            Maybe<object> expected = m;

            Assert.Equal(expected, actual);
        }

        //Return t >== f == f t
        [Property]
        public void LeftIdentityHolds(NonNull<object> x) {
            Func<object, Maybe<object>> f = Just;

            Maybe<object> expected = f(x.Get);
            Maybe<object> actual = Just(x.Get).Bind(f);

            Assert.Equal(expected, actual);
        }
    }
}
