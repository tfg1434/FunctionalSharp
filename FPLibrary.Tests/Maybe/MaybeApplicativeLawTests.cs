using System;
using System.Text.Json;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using static FPLibrary.F;

namespace FPLibrary.Tests.Maybe {
    public class MaybeApplicativeLawTests {
        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void ApplicativeIdentityHolds(Maybe<object> v) {
            Func<object, object> ident = x => x;
            Maybe<object> actual = Just(ident).Apply(v);
            Maybe<object> expected = v;

            Assert.Equal(expected, actual);
        }
    }
}
