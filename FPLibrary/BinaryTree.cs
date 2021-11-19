using System;

namespace FPLibrary; 

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

