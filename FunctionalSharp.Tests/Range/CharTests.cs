using System.Linq;
using Xunit;
using System.Collections.Generic;
using FunctionalSharp;
using static FunctionalSharp.F;

namespace FunctionalSharp.Tests.Range;

public class CharTests {
    [Fact]
    public void Range_From_Char() {
        IEnumerable<char> expected = new[] { 'a', 'b', 'c', 'd', 'e' };
        IEnumerable<char> actual = F.Range(from: 'a').Take(5);
        
        Assert.Equal(expected, actual);
        
        expected = new[] { 'x', 'w', 'v', 'u', 't' };
        actual = F.Range(from: 'x', second: 'w').Take(5);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Range_Char_IsBounded() {
        IEnumerable<char> range = F.Range((char) 0);

        Assert.Equal(char.MaxValue + 1, range.Count());
    }
}