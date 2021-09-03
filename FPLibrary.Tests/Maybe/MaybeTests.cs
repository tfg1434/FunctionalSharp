using System;
using FsCheck;
using Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;
using Unit = System.ValueTuple;

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
    }
}
