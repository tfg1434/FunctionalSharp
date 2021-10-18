using System.Linq;
using Xunit;
using System.Collections.Generic;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary.Tests.Range;

public class RangeTests {
    [Fact]
    public void Range_From_Int() {
        IEnumerable<int> expected = new[] { 1, 2, 3, 4, 5 };
        IEnumerable<int> actual = F.Range(from: 1).Take(5);
            
        Assert.Equal(expected, actual);

        expected = new[] { -5, -4, -3 };
        actual = F.Range(from: -5).Take(3);
            
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Range_FromTo_Int() {
        var expected = Enumerable.Empty<int>();
        IEnumerable<int> actual = F.Range(from: 5, to: 1);
            
        Assert.Equal(expected, actual);

        expected = new[] { -5 };
        actual = F.Range(from: -5, to: -5);
            
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Range_FromSecond_Int() {
        IEnumerable<int> expected = new[] { 5, 4, 3, 2, 1 };
        IEnumerable<int> actual = F.Range(from: 5, second: 4).Take(5);
            
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Range_FromSecondTo_Int() {
        IEnumerable<int> expected = new[] { 2, 7, 12 };
        IEnumerable<int> actual = F.Range(from: 2, second: 7, to: 15);
            
        Assert.Equal(expected, actual);

        expected = Enumerable.Empty<int>();
        actual = F.Range(from: 5, second: 4, to: 6);
        
        Assert.Equal(expected, actual);
    }
}