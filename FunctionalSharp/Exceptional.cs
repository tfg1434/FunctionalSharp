using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public static partial class F {
    /// <summary>
    /// Construct an <see cref="Exceptional{T}"/> from an <see cref="Exception"/> or <typeparamref name="T"/>
    /// </summary>
    [Pure]
    public static Exceptional<T> Exceptional<T>(T t) => new(t);
}

/// <summary>
/// Semantic version of Either&lt;Exception, T&gt;
/// </summary>
public readonly struct Exceptional<T> : IEquatable<Exceptional<T>> {
    private readonly Exception? _ex;
    private readonly T? _value;
    private readonly bool _isSucc;

    /// <summary>
    /// Exception ctor
    /// </summary>
    /// <param name="ex">Left exception</param>
    public Exceptional(Exception ex) {
        _isSucc = false;
        _ex = ex;
        _value = default;
    }

    /// <summary>
    /// Success ctor
    /// </summary>
    /// <param name="value">Success value</param>
    public Exceptional(T value) {
        _isSucc = true;
        _value = value;
        _ex = default;
    }

    [Pure]
    public static implicit operator Exceptional<T>(Exception ex) 
        => new(ex);

    [Pure]
    public static implicit operator Exceptional<T>(T t) 
        => new(t);

    /// <summary>
    /// Match the two states of the <see cref="Exceptional{T}"/>
    /// </summary>
    [Pure]
    public R Match<R>(Func<Exception, R> ex, Func<T, R> succ)
        => !_isSucc ? ex(_ex!) : succ(_value!);

    /// <summary>
    /// Side-effecting <see cref="Match{R}"/>
    /// </summary>
    public Unit Match(Action<Exception> ex, Action<T> succ)
        => Match(ex.ToFunc(), succ.ToFunc());

    /// <summary>
    /// Convert to <see cref="IEnumerable{T}"/> if in Success state
    /// </summary>
    [Pure]
    public IEnumerable<T> AsEnumerable() {
        if (_isSucc) yield return _value!;
    }
    
    /// <summary>
    /// Convert an <see cref="Exceptional{T}"/> to a <see cref="Maybe{T}"/> 
    /// </summary>
    [Pure]
    public Maybe<T> ToMaybe()
        => Match(_ => Nothing, Just);
    
    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public Exceptional<R> Map<R>(Func<T, R> f)
        => Match(
            ex => new Exceptional<R>(ex),
            t => f(t));

    /// <summary>
    /// Functor BiMap
    /// </summary>
    [Pure]
    public Exceptional<R> BiMap<R>(Func<Exception, Exception> ex, Func<T, R> succ)
        => Match<Exceptional<R>>(l => ex(l), r => succ(r));

    /// <summary>
    /// Side-effecting Map
    /// </summary>
    public Exceptional<Unit> ForEach(Action<T> act)
        => Map(act.ToFunc());

    /// <summary>
    /// Monadic Bind
    /// </summary>
    [Pure]
    public Exceptional<R> Bind<R>(Func<T, Exceptional<R>> f)
        => Match(ex => new(ex), f);
    
    /// <summary>
    /// Extract the value from the <see cref="Exceptional{T}"/> with a default value
    /// </summary>
    /// <param name="value">Default value</param>
    [Pure]
    public T IfFail(in T value)
        => _isSucc ? _value! : value;
    
    /// <summary>
    /// Extract the value from the <see cref="Exceptional{T}"/> with a lazy default value
    /// </summary>
    /// <param name="f">Lazy default value</param>
    [Pure]
    public T IfFail(Func<Exception, T> f)
        => _isSucc ? _value! : f(_ex!);

    /// <summary>
    /// Perform a side-effecting <see cref="Action"/> if the <see cref="Exceptional{T}"/> is in a Fail state
    /// </summary>
    /// <param name="f">Side-effecting <see cref="Action"/></param>
    /// <returns>Unit</returns>
    public Unit IfFail(Action<Exception> f) {
        if (!_isSucc) f(_ex!);

        return Unit();
    }
    
    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public Exceptional<R> Select<R>(Func<T, R> f)
        => Map(f);

    /// <summary>
    /// Monadic bind with a projection function
    /// </summary>
    [Pure]
    public Exceptional<RR> SelectMany<R, RR>(Func<T, Exceptional<R>> bind, Func<T, R, RR> proj)
        => Bind(t => bind(t).Map(r => proj(t, r)));
    
    #region Equality
    
    public bool Equals(Exceptional<T> other)
        => _isSucc && other._isSucc && EqualityComparer<T>.Default.Equals(_value, other._value) ||
           !_isSucc && !other._isSucc && EqualityComparer<Exception>.Default.Equals(_ex, other._ex);

    public override bool Equals(object? obj)
        => obj is Exceptional<T> other && Equals(other);

    public override int GetHashCode()
        => _isSucc
            ? _value!.GetHashCode()
            : _ex!.GetHashCode();
    
    [Pure]
    public static bool operator ==(Exceptional<T> self, Exceptional<T> other) 
        => self.Equals(other);

    [Pure]
    public static bool operator !=(Exceptional<T> self, Exceptional<T> other)
        => !(self == other);

    #endregion
    
    [Pure]
    public override string ToString()
        => Match(
            exception => $"Exception({exception.Message})",
            t => $"Success({t})");
}

public static class Exceptional {
    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Exceptional<R> Apply<T, R>(this Exceptional<Func<T, R>> self, Exceptional<T> arg)
        => self.Match(
            ex => ex,
            func => arg.Match(
                ex => ex,
                t => Exceptional(func(t))));

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Exceptional<Func<T2, R>> Apply<T1, T2, R>(this Exceptional<Func<T1, T2, R>> self,
        Exceptional<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Exceptional<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Exceptional<Func<T1, T2, T3, R>> self,
        Exceptional<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Exceptional<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(
        this Exceptional<Func<T1, T2, T3, T4, R>> self, Exceptional<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Exceptional<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>(
        this Exceptional<Func<T1, T2, T3, T4, T5, R>> self, Exceptional<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Exceptional<Func<T2, T3, T4, T5, T6, R>> Apply<T1, T2, T3, T4, T5, T6, R>(
        this Exceptional<Func<T1, T2, T3, T4, T5, T6, R>> self, Exceptional<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Exceptional<Func<T2, T3, T4, T5, T6, T7, R>> Apply<T1, T2, T3, T4, T5, T6, T7, R>(
        this Exceptional<Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Exceptional<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);
}