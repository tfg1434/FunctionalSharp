using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using Xunit;

namespace FPLibrary.Tests {
    public static class Utils {
        public static void Fail() => Assert.True(false);
        public static void Succeed() => Assert.True(true);

        //just random numbers
        public static readonly Func<int, int> Times2 = x => x * 2;
        public static readonly Func<int, int> Plus5 = x => x + 5;
        public static readonly Func<int, int> Plus7 = x => x + 7;

        public static readonly Func<int, int, int> Add = (i, j) => i + j;
        public static readonly Func<int, int, int, int> Add3 =
            (a, b, c) => a + b + c;
        public static readonly Func<int, int, int, int, int> Add4 =
            (a, b, c, d) => a + b + c + d;
        public static readonly Func<int, int, int, int, int, int> Add5 =
            (a, b, c, d, e) => a + b + c + d + e;
        public static readonly Func<int, int, int, int, int, int, int> Add6 =
            (a, b, c, d, e, f) => a + b + c + d + e + f;
        public static readonly Func<int, int, int, int, int, int, int, int> Add7 =
            (a, b, c, d, e, f, g) => a + b + c + d + e + f + g;
    }
    
    public static class ArbitraryIEnumerable {
        private static Gen<IEnumerable<T>> Empty<T>()
            => Gen.Constant(Enumerable.Empty<T>());

        private static Gen<IEnumerable<T>> NonEmpty<T>()
            => from head in Arb.Generate<T>()
               from tail in GenIEnumerable<T>()
               select ImmutableList.Create(head)
                   .Concat(tail);

        private static Gen<IEnumerable<T>> GenIEnumerable<T>()
            => from isEmpty in Arb.Generate<bool>()
               from list in isEmpty ? Empty<T>() : NonEmpty<T>()
               select list;

        public static Arbitrary<IEnumerable<T>> IEnumerable<T>()
            => GenIEnumerable<T>().ToArbitrary();
    }
}
