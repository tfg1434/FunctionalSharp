using System;
using System.Text.Json;
using System.Xml.Schema;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.Maybe {
    public class MaybeApplicativeLawTests {
        //Return x <*> v == v
        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void ApplicativeIdentityHolds(Maybe<object> v) {
            Func<object, object> ident = x => x;
            Maybe<object> actual = Just(ident).Apply(v);
            Maybe<object> expected = v;

            Assert.Equal(expected, actual);
        }

        //Return x <*> u <*> v <*> w == u <*> (v <*> w)
        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void CompositionHolds(Maybe<int> w, bool uIsJust, bool vIsJust) {
            Func<Func<int, int>, Func<int, int>, Func<int, int>> compose
                = (f, g) => x => f(g(x));

            Maybe<Func<int, int>> u = uIsJust ? Just(times2) : Nothing;
            Maybe<Func<int, int>> v = vIsJust ? Just(plus5) : Nothing;

            Maybe<int> expected = Just(compose)
                .Apply(u)
                .Apply(v)
                .Apply(w);
            Maybe<int> actual = u.Apply(v.Apply(w));

            Assert.Equal(expected, actual);
        }

        //Return f <*> Return x == Return $ f x
        [Property]
        public void HomomorphismHolds(int x) {
            Maybe<int> expected = Just(times2).Apply(Just(x));
            Maybe<int> actual = Just(times2(x));

            Assert.Equal(expected, actual);
        }
    }
}
