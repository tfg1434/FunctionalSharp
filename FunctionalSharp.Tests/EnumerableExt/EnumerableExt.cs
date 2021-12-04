using System.Collections.Generic;
using FunctionalSharp;
using static FunctionalSharp.F;

namespace FunctionalSharp.Tests.EnumerableExt;

public class EnumerableExtTests {
    [Property(Arbitrary = new[] { typeof(ArbitraryIEnumerable) })]
    public void ForEach_Incr_Test(IEnumerable<int> enumerable) {
        int i = 0;
        
        enumerable.ForEach(x => i += x);
        Assert.Equal(enumerable.Sum(), i);
    }
    
    [Fact]
    public void First_Maybe_Test() {
        Assert.Equal(Just(1), List(1, 2, 3).First(_ => true));
        Assert.Equal(Nothing, Enumerable.Empty<int>().First(_ => true));
    }

    [Fact]
    public void Flatten_Equals_BindIdent() {
        Assert.Equal(List(1, 2, 3, 4).AsEnumerable(), 
            List(List(1, 2).AsEnumerable(), List(3, 4).AsEnumerable()).Flatten());
        Assert.Equal(List(List(1, 2).AsEnumerable(), List(3, 4).AsEnumerable()).Flatten(),
            List(List(1, 2).AsEnumerable(), List(3, 4).AsEnumerable()).Bind(x => x));
    }
    
    [Fact]
    public void Match_Cons_Empty() {
        Assert.Equal(1, List(1, 2, 3).AsEnumerable().Match(() => 0, (x, _) => x));
        Assert.Equal(0, Enumerable.Empty<int>().Match(() => 0, (x, _) => x));
    }

    [Fact]
    public void Match_List_Empty() {
        Assert.Equal(List(1, 2, 3).AsEnumerable(), 
            List(1, 2, 3).AsEnumerable().Match(() => List(0), xs => xs));
        Assert.Equal(0, Enumerable.Empty<int>().Match(() => 0, _ => 1));
    }
}