using System;
using Xunit;
using FPLibrary;
using FsCheck.Xunit;
using FsCheck;
using static FPLibrary.F;

namespace FPLibrary.Tests.Maybe {
    public class ConversionTests {
        [Fact]
        public void Implicit_Null_Nothing() {
            Maybe<string> expected = Nothing;
            Maybe<string> actual = null;

            Assert.Equal(expected, actual);
        }

        [Property]
        public void Implicit_NotNull_Just(NonNull<string> str) {
            Maybe<string> expected = Just(str.Get);
            Maybe<string> actual = str.Get;

            Assert.Equal(expected, actual);
        }
    }
}
