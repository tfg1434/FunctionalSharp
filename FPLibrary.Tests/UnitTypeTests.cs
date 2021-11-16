using System;
using Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;
using Unit = System.ValueTuple;

namespace FPLibrary.Tests {
    public class UnitType {
        [Fact]
        public void Unit_OnlyOne() {
            Assert.Equal(new Unit(), Unit());
        }
    }
}
