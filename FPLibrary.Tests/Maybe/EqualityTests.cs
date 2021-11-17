using FsCheck.Xunit;
using Xunit;
using static FPLibrary.F;

namespace FPLibrary.Tests.Maybe; 

public class EqualityTests {
    [Fact]
    public void Equal_HashCode_Holds() {
        var a = Just(1);
        var b = Just(1);
        
        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
    public void HashCode_NoThrow_Holds(Maybe<int> m) {
        m.GetHashCode();
    }

    [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
    public void Equals_Is_Equal(int n) {
        var a = Just(n);
        var b = Just(n);
        
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a, b);
    }
}