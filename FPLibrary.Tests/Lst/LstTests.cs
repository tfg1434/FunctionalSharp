using System;
using System.Linq;
using Xunit;
using static FPLibrary.F;

namespace FPLibrary.Tests.Lst; 

public class LstTests {
    [Fact]
    public void Empty_List_Equality() {
        Assert.Empty(Lst<int>.Empty);
        Assert.Equal(Lst<int>.Empty, Lst<int>.Empty);
        Assert.Equal(default, Lst<int>.Empty);
    }

    [Fact]
    public void Head_Returns_FirstElement() {
        Assert.Equal(1, List(1, 2, 3).Head);
        Assert.Throws<InvalidOperationException>(() => Lst<int>.Empty.Head);
        
        Assert.Equal(Just(1), List(1, 2, 3).HeadSafe);
        Assert.Equal(Nothing, Lst<int>.Empty.HeadSafe);
    }
    
    [Fact]
    public void Tail_Returns_AllButFirstElement() {
        Assert.Equal(List(2, 3), List(1, 2, 3).Tail);
        Assert.Throws<InvalidOperationException>(() => Lst<int>.Empty.Tail);
        
        Assert.Equal(Just(List(2, 3)), List(1, 2, 3).TailSafe);
        Assert.Equal(Nothing, Lst<int>.Empty.TailSafe);
    }
    
    [Fact]
    public void Match_Returns_MatchingValue() {
        Assert.Equal(1, List(1, 2, 3).Match(() => 0, (x, _) => x));
        Assert.Equal(0, Lst<int>.Empty.Match(() => 0, (x, _) => x));
    }

    [Fact]
    public void Enumerator_Enumeration_Works() {
        Lst<int> list = List(1, 2, 3);
        string str = "";
        list.ForEach(t => str += $"{t},");
        
        Assert.Equal("1,2,3,", str);

        using var enumerator = Lst<string>.Empty.GetEnumerator();
        Assert.Null(enumerator.Current);
        Assert.False(enumerator.MoveNext());
    }
    
    [Fact]
    public void Contains_Returns_Exists() {
        Assert.False(Lst<int>.Empty.Contains(1));
        Assert.True(List(1, 2, 3).Contains(1));
    }
    
    [Fact]
    public void Prepend_Returns_ListWithAdded() {
        Assert.True(Lst<int>.Empty.Prepend(1).Prepend(2).SequenceEqual(List(2, 1)));
        Assert.True(Lst<int>.Empty.Prepend(new[] { 2, 1 }).SequenceEqual(List(2, 1)));
    }
    
    [Fact]
    public void Append_Returns_ListWithAdded() {
        Assert.Equal(List(1, 2, 3), Lst<int>.Empty.Append(1).Append(2).Append(3));
        Assert.Equal(List(1, 2, 3, 4), List(1).Append(new[] { 2, 3, 4 }));
        Assert.Equal(List(1, 2, 3, 4), List(1).Append(List(2, 3, 4)));
    }

    [Fact]
    public void Remove_AtAll_Removes() {
        Assert.Empty(Lst<int>.Empty.Remove(0));
        Assert.Equal(List(1, 3), List(1, 2, 3).Remove(2));

        Assert.Throws<ArgumentOutOfRangeException>(() => List(1, 2, 3).RemoveAt(-1));
        Assert.Equal(List(1, 2, 3), List(1, 2, 3, 4).RemoveAt(3));
        
        Assert.Equal(List(1, 2, 3), List(Enumerable.Range(1, 10)).RemoveAll(x => x > 3));
    }

    [Fact]
    public void Skip_Skips_EqualsEnumerable() {
        Assert.Equal(new[] { 1 }.Skip(-1), List(1).Skip(-1));
        Assert.Equal(new[] { 1 }.Skip(2), List(1).Skip(2));
    }
    
    [Fact]
    public void Skip_While_Skips() {
        Assert.Empty(Lst<int>.Empty.SkipWhile(_ => true));
        Assert.Equal(List(1, 2, 3), List(1, 2, 3).SkipWhile(_ => false));
        
        Assert.Empty(Lst<int>.Empty.SkipWhile((_, index) => true));
    }

    [Fact]
    public void Index_Is_Supported() {
        Assert.Equal(3, List(1, 2, 3)[^1]);
        Assert.Equal(List(1, 2, 3), List(1, 2, 3)[0..^0]);
        Assert.Equal(List(1, 2), List(1, 2, 3)[..^1]);
    }
}