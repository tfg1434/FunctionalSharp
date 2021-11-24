using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp;

using static F;

public static partial class F {
    /// <summary>
    /// Construct an <see cref="Either{L,R}"/> in Left state
    /// </summary>
    [Pure]
    public static Either.Left<L> Left<L>(L left) => new(left);

    /// <summary>
    /// Construct an <see cref="Either{L,R}"/> in Right state
    /// </summary>
    [Pure]
    public static Either.Right<R> Right<R>(R right) => new(right);
}

/// <summary>
/// Either is a discriminated union that can either be Left or Right state. Left represents failure, while Right
/// represents success
/// </summary>
public readonly struct Either<L, R> : IEquatable<Either<L, R>> {
    private readonly L? _left;
    private readonly R? _right;

    private readonly bool _isRight;
        
    internal Either(L l) {
        _isRight = false;
        _left = l;
        _right = default;
    }

    internal Either(R r) {
        _isRight = true;
        _left = default;
        _right = r;
    }

    [Pure]
    public static implicit operator Either<L, R>(L left) => new(left);

    [Pure]
    public static implicit operator Either<L, R>(R right) => new(right);

    [Pure]
    public static implicit operator Either<L, R>(Either.Left<L> left) => new(left.Value);

    [Pure]
    public static implicit operator Either<L, R>(Either.Right<R> right) => new(right.Value);

    /// <summary>
    /// Match the two states of the <see cref="Either{L,R}"/>
    /// </summary>
    [Pure]
    public TR Match<TR>(Func<L, TR> l, Func<R, TR> r)
        => _isRight ? r(_right!) : l(_left!);

    /// <summary>
    /// Side effecting <see cref="Match{TR}"/>
    /// </summary>
    [Pure]
    public Unit Match(Action<L> l, Action<R> r)
        => Match(l.ToFunc(), r.ToFunc());

    /// <summary>
    /// Convert to <see cref="IEnumerable{T}"/> if in Right state
    /// </summary>
    [Pure]
    public IEnumerable<R> AsEnumerable() {
        if (_isRight) yield return _right!;
    }

    [Pure]
    public override string ToString() 
        => Match(l => $"Left({l})", r => $"Right({r})");
}

public static class Either {
    /// <summary>
    /// Left is one state of <see cref="Either{L,R}"/>. To use this type, use the methods in <see cref="Either{L,R}"/>
    /// </summary>
    public readonly struct Left<L> {
        internal L Value { get; }

        internal Left(L value) 
            => Value = value;

        [Pure]
        public override string ToString() 
            => $"Left({Value})";
    }

    /// <summary>
    /// Right is one state of <see cref="Either{L,R}"/>. To use this type, use the methods in <see cref="Either{L,R}"/>
    /// </summary>
    public readonly struct Right<R> {
        internal R Value { get; }

        internal Right(R value) 
            => Value = value;

        [Pure]
        public override string ToString() 
            => $"Right({Value})";
    }
}

/// <summary>
/// Extensions for <see cref="Either{L,R}"/>
/// </summary>
public static class EitherExt {
    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public static Either<L, RR> Map<L, R, RR>(this Either<L, R> self, Func<R, RR> f)
        // ReSharper disable once ConvertClosureToMethodGroup
        => self.Match<Either<L, RR>>(l => Left(l), r => Right(f(r)));

    /// <summary>
    /// Functor BiMap
    /// </summary>
    [Pure]
    public static Either<LL, RR> BiMap<L, LL, R, RR>(this Either<L, R> self, Func<L, LL> left, Func<R, RR> right)
        => self.Match<Either<LL, RR>>(l => Left(left(l)), r => Right(right(r)));

    /// <summary>
    /// Side-effecting Map
    /// </summary>
    public static Either<L, Unit> ForEach<L, R>(this Either<L, R> either, Action<R> act)
        => either.Map(act.ToFunc());

    /// <summary>
    /// Monadic Bind
    /// </summary>
    [Pure]
    public static Either<L, RR> Bind<L, R, RR>(this Either<L, R> either, Func<R, Either<L, RR>> f)
        // ReSharper disable once ConvertClosureToMethodGroup
        => either.Match(l => Left(l), f);

    /// <summary>
    /// Convert to <see cref="Just{T}"/> or <see cref="F.Nothing"/> depending on state of this <see cref="Either{L,R}"/>
    /// </summary>
    [Pure]
    public static Maybe<R> ToMaybe<L, R>(this Either<L, R> self)
        => self.Match(_ => Nothing, Just);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Either<L, RR> Apply<L, R, RR>(this Either<L, Func<R, RR>> self, Either<L, R> arg)
        => self.Match(
            // ReSharper disable once ConvertClosureToMethodGroup
            l => Left(l),
            f => arg.Match<Either<L, RR>>(
                // ReSharper disable once ConvertClosureToMethodGroup
                l => Left(l),
                t => Right(f(t))));

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Either<L, Func<T2, R>> Apply<L, T1, T2, R>(this Either<L, Func<T1, T2, R>> self,
        Either<L, T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Either<L, Func<T2, T3, R>> Apply<L, T1, T2, T3, R>(this Either<L, Func<T1, T2, T3, R>> self,
        Either<L, T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Either<L, Func<T2, T3, T4, R>> Apply<L, T1, T2, T3, T4, R>(
        this Either<L, Func<T1, T2, T3, T4, R>> self, Either<L, T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Either<L, Func<T2, T3, T4, T5, R>> Apply<L, T1, T2, T3, T4, T5, R>(
        this Either<L, Func<T1, T2, T3, T4, T5, R>> self, Either<L, T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Either<L, Func<T2, T3, T4, T5, T6, R>> Apply<L, T1, T2, T3, T4, T5, T6, R>(
        this Either<L, Func<T1, T2, T3, T4, T5, T6, R>> self, Either<L, T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Either<L, Func<T2, T3, T4, T5, T6, T7, R>> Apply<L, T1, T2, T3, T4, T5, T6, T7, R>(
        this Either<L, Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Either<L, T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public static Either<L, RR> Select<L, R, RR>(this Either<L, R> self, Func<R, RR> f)
        => self.Map(f);

    /// <summary>
    /// Monadic bind with a projection
    /// </summary>
    [Pure]
    public static Either<L, RRR> SelectMany<L, R, RR, RRR>(this Either<L, R> self, Func<R, Either<L, RR>> bind,
        Func<R, RR, RRR> proj)
        => self.Bind(t => bind(t).Map(r => proj(t, r)));
}