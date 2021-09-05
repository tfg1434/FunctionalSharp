using System;
using Xunit;

namespace FPLibrary.Tests {
    public static class Utils {
        public static void Fail() => Assert.True(false);
        public static void Succeed() => Assert.True(true);

        public static readonly Func<int, int> Plus5 = x => x + 5;
        public static readonly Func<int, int> Times2 = x => x * 2;
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
}
