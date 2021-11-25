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
    
    /// <summary>
    /// Convert an <see cref="Either{L,R}"/> to a <see cref="Maybe{T}"/> 
    /// </summary>
    [Pure]
    public Maybe<R> ToMaybe()
        => Match(_ => Nothing, Just);
    
    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public Either<L, RR> Map<RR>(Func<R, RR> f)
        // ReSharper disable once ConvertClosureToMethodGroup
        => Match<Either<L, RR>>(l => Left(l), r => Right(f(r)));
    
    /// <summary>
    /// Functor BiMap
    /// </summary>
    [Pure]
    public Either<LL, RR> BiMap<LL, RR>(Func<L, LL> left, Func<R, RR> right)
        => Match<Either<LL, RR>>(l => Left(left(l)), r => Right(right(r)));
    
    /// <summary>
    /// Side-effecting Map
    /// </summary>
    public Either<L, Unit> ForEach(Action<R> act)
        => Map(act.ToFunc());
    
    /// <summary>
    /// Monadic Bind
    /// </summary>
    [Pure]
    public Either<L, RR> Bind<RR>(Func<R, Either<L, RR>> f)
        // ReSharper disable once ConvertClosureToMethodGroup
        => Match(l => Left(l), f);
    
    /// <summary>
    /// Extract the value from the <see cref="Either{L,R}"/> with a default value
    /// </summary>
    /// <param name="right">Default value</param>
    [Pure]
    public R IfFail(in R right)
        => _isRight ? _right! : right;
    
    /// <summary>
    /// Extract the value from the <see cref="Either{L,R}"/> with a lazy default value
    /// </summary>
    /// <param name="f">Lazy default value</param>
    [Pure]
    public R IfFail(Func<L, R> f)
        => _isRight ? _right! : f(_left!);

    /// <summary>
    /// Perform a side-effecting <see cref="Action"/> if the <see cref="Either{L,R}"/> is in a Fail state
    /// </summary>
    /// <param name="f">Side-effecting <see cref="Action"/></param>
    /// <returns>Unit</returns>
    public Unit IfFail(Action<L> f) {
        if (!_isRight) f(_left!);

        return Unit();
    }
    
    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public Either<L, RR> Select<RR>(Func<R, RR> f)
        => Map(f);

    /// <summary>
    /// Monadic bind with a projection
    /// </summary>
    [Pure]
    public Either<L, PR> SelectMany<RR, PR>(Func<R, Either<L, RR>> bind, Func<R, RR, PR> proj)
        => Bind(t => bind(t).Map(r => proj(t, r)));
    
    #region Equality
    
    public bool Equals(Either<L, R> other)
        => _isRight && other._isRight && EqualityComparer<R>.Default.Equals(_right, other._right) ||
           !_isRight && !other._isRight && EqualityComparer<L>.Default.Equals(_left, other._left);

    public override bool Equals(object? obj)
        => obj is Either<L, R> other && Equals(other);

    public override int GetHashCode()
        => _isRight
            ? _left!.GetHashCode()
            : _right!.GetHashCode();
    
    [Pure]
    public static bool operator ==(Either<L, R> self, Either<L, R> other) 
        => self.Equals(other);

    [Pure]
    public static bool operator !=(Either<L, R> self, Either<L, R> other)
        => !(self == other);

    #endregion

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
}