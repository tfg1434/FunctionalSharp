using Xunit;
using static FPLibrary.F;

namespace FPLibrary.Tests.Lst; 

public class EqualityTests {
    [Fact]
    public void Equal_HashCode_Holds() {
        var list1 = List(1, 2, 3);
        var list2 = List(1, 2, 3);
        
        Assert.Equal(list1, list2);
        Assert.Equal(list1.GetHashCode(), list2.GetHashCode());
    }

    [Fact]
    public void HashCode_Order_Matters() {
        int expected = List(1, 2, 3).GetHashCode();
        int actual = List(3, 2, 1).GetHashCode();
        
        Assert.NotEqual(expected, actual);
    }
    
    [Fact]
    public void HashCode_Duplicates_Matters() {
        int expected = List(1, 2, 3).GetHashCode();
        int actual = List(1, 2, 3, 3).GetHashCode();
        
        Assert.NotEqual(expected, actual);
    }
    
    [Fact]
    public void Equals_Comparer_Uses() {
        //TODO: Implement
    }
}