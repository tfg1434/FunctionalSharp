using System;
using System.Collections.Specialized;
using FsCheck;
using Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;
using Unit = System.ValueTuple;
using FPLibrary;
using FsCheck.Xunit;

namespace FPLibrary.Tests.Maybe {
    static class ArbitraryMaybe {
        public static Arbitrary<Maybe<T>> Maybe<T>() {
            var gen = from isJust in Arb.Generate<bool>()
                from val in Arb.Generate<T>()
                select isJust && val is not null ? Just(val) : Nothing;

            return gen.ToArbitrary();
        }
    }

    public class MaybeTests {
        [Fact]
        public void Match_Int_Just() {
            GetValue(true)
                .Match(
                    Fail,
                    _ => Succeed());
        }

        [Fact]
        public void Match_String_Nothing() {
            GetValue(false)
                .Match(
                    Succeed,
                    _ => Fail());
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void LinqQuery_SingleClause_Just(Maybe<int> m) {
            Maybe<int> expected = m.Map(times2);
            Maybe<int> actual = from x in m 
                                select times2(x);
            
            Assert.Equal(expected, actual);
        }

        private Maybe<int> GetValue(bool select)
            => select ? Just(0) : Nothing;
    }
}
