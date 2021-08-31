using System;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using static FPLibrary.F;

namespace FPLibrary.Tests.Maybe {
    static class ArbitraryMaybe {
        public static Arbitrary<Maybe<T>> Maybe<T>() {
            var gen = from isJust in Arb.Generate<bool>()
                      from val in Arb.Generate<T>()
                      select isJust ? Just(val) : Nothing;

            return gen.ToArbitrary();
        }
    }

    public class MaybeApplicativeLawTests {
        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void IdentityHolds(Maybe<int> v) {
            Func<int, int> ident = x => x;

            Assert.Equal(Just(ident).Apply(v), v);
        }


    }
}
