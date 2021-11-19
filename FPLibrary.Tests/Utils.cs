using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using Xunit;
#pragma warning disable CS8619

namespace FPLibrary.Tests; 

public static class Utils {
    public static void Fail() => Assert.True(false);
    public static void Succeed() => Assert.True(true);

    //just random numbers
    public static readonly Func<int, int> Times2 = x => x * 2;
    public static readonly Func<int, int> Plus5 = x => x + 5;
    public static readonly Func<int, int> Plus7 = x => x + 7;

    public static readonly Func<int, int, int> Add = (i, j) => i + j;
    public static readonly Func<int, int, int, int> Add3 =
        (a, b, c) => a + b + c;
    public static readonly Func<int, int, int, int, int> Add4 =
        (a, b, c, d) => a + b + c + d;
    public static readonly Func<int, int, int, int, int, int> Add5 =
        (a, b, c, d, e) => a + b + c + d + e;
    public static readonly Func<int, int, int, int, int, int, int> Add6 =
        (a, b, c, d, e, f) => a + b + c + d + e + f;
    public static readonly Func<int, int, int, int, int, int, int, int> Add7 =
        (a, b, c, d, e, f, g) => a + b + c + d + e + f + g;
}
    
public static class ArbitraryIEnumerable {
    private static Gen<IEnumerable<T>> Empty<T>()
        => Gen.Constant(Enumerable.Empty<T>());

    private static Gen<IEnumerable<T>> NonEmpty<T>()
        => from head in Arb.Generate<T>()
           from tail in GenIEnumerable<T>()
           select ImmutableList.Create(head)
               .Concat(tail);

    private static Gen<IEnumerable<T>> GenIEnumerable<T>()
        => from isEmpty in Arb.Generate<bool>()
           from list in isEmpty ? Empty<T>() : NonEmpty<T>()
           select list;

    public static Arbitrary<IEnumerable<T>> IEnumerable<T>()
        => GenIEnumerable<T>().ToArbitrary();
}
    
public abstract record Tree<T>;

internal record Leaf<T>(T Value) : Tree<T>;

internal record Branch<T>(Tree<T> Left, Tree<T> Right) : Tree<T>;

public static class Tree {
    public static R Match<T, R>(this Tree<T> tree, Func<T, R> leaf, Func<Tree<T>, Tree<T>, R> branch) => 
        tree switch {
            Leaf<T>({ } value) => leaf(value),
            Branch<T>({ } l, { } r) => branch(l, r),
            _ => throw new ArgumentException("Invalid tree"),
        };

    public static Tree<T> Leaf<T>(T value) => new Leaf<T>(value);
    
    public static Tree<T> Branch<T>(Tree<T> left, Tree<T> right) => new Branch<T>(left, right);

    public static Tree<R> Map<T, R>(this Tree<T> self, Func<T, R> f)
        => self.Match(
            leaf: t => Leaf(f(t)),
            branch: (l, r) => Branch(l.Map(f), r.Map(f)));
    
    public static int Count<T>(this Tree<T> self)
        => self.Match(
            leaf: _ => 1,
            branch: (l, r) => l.Count() + r.Count());
    
    public static int Height<T>(this Tree<T> self)
        => self.Match(
            leaf: _ => 0,
            branch: (l, r) => 1 + Math.Max(l.Height(), r.Height()));

    public static Tree<T> Insert<T>(this Tree<T> self, T value)
        => self.Match(
            leaf: t => Branch(Leaf(t), Leaf(value)),
            branch: (l, r) => Branch(l, r.Insert(value)));

    public static T Aggregate<T>(this Tree<T> self, Func<T, T, T> f)
        => self.Match(
            leaf: t => t,
            branch: (l, r) => f(l.Aggregate(f), r.Aggregate(f)));

    public static A Aggregate<T, A>(this Tree<T> self, A acc, Func<A, T, A> f)
        => self.Match(
            leaf: t => f(acc, t),
            branch: (l, r) => {
                A leftAcc = l.Aggregate(acc, f);

                return r.Aggregate(leftAcc, f);
            });
}