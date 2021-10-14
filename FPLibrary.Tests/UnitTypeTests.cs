using System;
using Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;
using Unit = System.ValueTuple;

namespace FPLibrary.Tests {
    public class UnitTypeTests {
        [Fact]
        public void Unit_OnlyOne() {
            Assert.Equal(new Unit(), Unit());
        }
    }
}
