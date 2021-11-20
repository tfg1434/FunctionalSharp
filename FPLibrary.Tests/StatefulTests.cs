using Xunit;

namespace FPLibrary.Tests; 
using static FPLibrary.Tests.Utils;

public class StatefulTests {
    public record Numbered<T>(int Number, T Value);

    private static Stateful<int, int> GetAndIncr => count => (count, count + 1);

    Stateful<int, Tree<Numbered<T>>> Number<T>(Tree<T> tree)
        => tree.Match(
            leaf: t =>
                from count in GetAndIncr
                select Tree.Leaf(new Numbered<T>(count, t)),
            branch: (l, r) =>
                from newL in Number(l)
                from newR in Number(r)
                select Tree.Branch(newL, newR));

    [Fact]
    public void NumberedTest() {
        Tree<int> tree = 
            new Branch<int>(
            new Leaf<int>(1), new Branch<int>(
                new Leaf<int>(2), new Leaf<int>(3)));

        Tree<Numbered<int>> numbered = Number(tree).Run(0);

        //3 = 0 + 1 + 2
        Assert.Equal(3, numbered.Aggregate(0, (acc, node) => acc + node.Number));
    }
}