using System;
using Xunit;

namespace FPLibrary.Tests {
    public static class Utils {
        public static void Fail() => Assert.True(false);

        public static Func<int, int> plus5 = x => x + 5;
        public static Func<int, int> times2 = x => x * 2;
    }
}
