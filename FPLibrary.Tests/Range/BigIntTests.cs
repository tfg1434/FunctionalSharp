using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace FPLibrary.Tests.Range; 

public class BigIntegerTests {
    [Fact]
    public void Range_From_BigInteger() {
        IEnumerable<BigInteger> expected = new BigInteger[] { 1, 2, 3, 4, 5 };
        IEnumerable<BigInteger> actual = F.Range(from: new BigInteger(1)).Take(5);
            
        Assert.Equal(expected, actual);

        expected = new BigInteger[] { -5, -4, -3 };
        actual = F.Range(from: new BigInteger(-5)).Take(3);
            
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Range_FromTo_BigInteger() {
        var expected = Enumerable.Empty<BigInteger>();
        IEnumerable<BigInteger> actual = F.Range(from: new BigInteger(5), to: new(1));
            
        Assert.Equal(expected, actual);

        expected = new BigInteger[] { -5 };
        actual = F.Range(from: new BigInteger(-5), to: new(-5));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Range_FromSecond_BigInteger() {
        IEnumerable<BigInteger> expected = new BigInteger[] { 5, 4, 3, 2, 1 };
        IEnumerable<BigInteger> actual = F.Range(from: new BigInteger(5), second: new(4)).Take(5);
            
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Range_FromSecondTo_BigInteger() {
        IEnumerable<BigInteger> expected = new BigInteger[] { 2, 7, 12 };
        IEnumerable<BigInteger> actual = F.Range(from: new BigInteger(2), second: new(7), to: new(15));
            
        Assert.Equal(expected, actual);

        expected = Enumerable.Empty<BigInteger>();
        actual = F.Range(from: new BigInteger(5), second: new(4), to: new(6));
        
        Assert.Equal(expected, actual);
    }
}