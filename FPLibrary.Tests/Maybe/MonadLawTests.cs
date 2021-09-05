using System;
using Xunit;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.Maybe {
    public class MonadLawTests {
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
            object obj = x.Get;

            Maybe<object> expected = Just(obj);
            Maybe<object> actual = Just(obj).Bind(Just);

            Assert.Equal(expected, actual);
        }

        //(m >>= f) >>= g == m >>= (x => f(x) >>= g)
        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void AssociativeHolds(Maybe<int> m) {

            Func<int, Maybe<int>> f = x => Just(Times2(x));
            Func<int, Maybe<int>> g = x => Just(Plus5(x));

            Maybe<int> expected = m.Bind(f).Bind(g);
            Maybe<int> actual = m.Bind(x => f(x).Bind(g));

            Assert.Equal(expected, actual);
        }
    }
}
