using System;
using Xunit;

namespace FPLibrary.Tests {
    public static class Utils {
        public static void Fail() => Assert.True(false);
        public static void Succeed() => Assert.True(true);

        public static readonly Func<int, int> Plus5 = x => x + 5;
        public static readonly Func<int, int> Times2 = x => x * 2;
        public static readonly Func<int, int, int> Multiply = (i, j) => i * j;
    }
}
