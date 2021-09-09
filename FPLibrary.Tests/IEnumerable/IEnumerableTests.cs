using System;
using FsCheck;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace FPLibrary.Tests.IEnumerable {
    static class ArbitraryIEnumerable {
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
