using System;
using System.Text.Json;
using System.Xml.Schema;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;
using FPLibrary;

namespace FPLibrary.Tests.Maybe {
    public class ApplicativeLawTests {
        //Return x <*> v == v
        [Property(Arbitrary = new[] {typeof(ArbitraryMaybe)})]
        public void IdentityHolds(Maybe<object> v) {
            Func<object, object> ident = x => x;
            Maybe<object> actual = Just(ident).Apply(v);
            Maybe<object> expected = v;

            Assert.Equal(expected, actual);
        }

        //Return x <*> u <*> v <*> w == u <*> (v <*> w)
        [Property(Arbitrary = new[] {typeof(ArbitraryMaybe)})]
        public void CompositionHolds(Maybe<int> w, bool uIsJust, bool vIsJust) {
            Func<Func<int, int>, Func<int, int>, Func<int, int>> compose
                = (f, g) => x => f(g(x));

            Maybe<Func<int, int>> u = uIsJust ? Just(Times2) : Nothing;
            Maybe<Func<int, int>> v = vIsJust ? Just(Plus5) : Nothing;

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
            Maybe<int> expected = Just(Times2).Apply(Just(x));
            Maybe<int> actual = Just(Times2(x));

            Assert.Equal(expected, actual);
        }

        //u <*> Return y == Return ($ y) <*> u
        //($ y) takes a function and applies it to y like (+4)
        //($ y) == \f -> f $ y
        [Property]
        public void InterchangeHolds(bool uIsJust, int y) {
            Maybe<Func<int, int>> u = uIsJust ? Just(Times2) : Nothing;

            Maybe<int> expected = u.Apply(Just(y));
            Maybe<int> actual = 
                Just<Func<Func<int, int>, int>>(f => f(y))
                .Apply(u);

            Assert.Equal(expected, actual);
        }
    }
}
