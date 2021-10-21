using System.Linq;
using Xunit;
using System.Collections.Generic;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary.Tests.Range;

public class CharTests {
    [Fact]
    public void Range_From_Char() {
        IEnumerable<char> expected = new[] { 'a', 'b', 'c', 'd', 'e' };
        IEnumerable<char> actual = F.Range(from: 'a').Take(5);
        
        Assert.Equal(expected, actual);
        
        expected = new[] { 'B', 'Å', 'ň', 'ǋ', 'Ɏ' };
        actual = F.Range(from: 'B', second: 'A').Take(5);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Range_Char_IsBounded() {
        IEnumerable<char> range = F.Range((char) 0);
        var r = ((Range<char>)F.Range((char)0));

        Assert.Equal(range.Count(), char.MaxValue);
    }
}