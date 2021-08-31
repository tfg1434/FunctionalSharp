using System;
using Xunit;

namespace FPLibrary.Tests {
    public static class Utils {
        public static void Fail() => Assert.True(false);
    }
}
