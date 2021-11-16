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
    
    // [Fact]
}