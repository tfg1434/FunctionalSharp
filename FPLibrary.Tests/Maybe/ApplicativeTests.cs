using System;
using Xunit;
using FsCheck.Xunit;
using FsCheck;
using FPLibrary;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.Maybe {
    public class ApplicativeTests {
        [Property]
        public void Apply_Nothing_Nothing(int i) {
            Maybe<int> m = Just(i)
                .Map(Multiply)
                .Apply(Nothing);
        }
    }
}
