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
public readonly struct Exceptional<T> {
    private readonly Exception? _ex;
    private readonly T? _value;

    private readonly bool _isSucc;

    internal Exceptional(Exception ex) {
        _isSucc = false;
        _ex = ex;
        _value = default;
    }

    internal Exceptional(T value) {
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
    [Pure]
    public Unit Match(Action<Exception> ex, Action<T> succ)
        => Match(ex.ToFunc(), succ.ToFunc());

    [Pure]
    public override string ToString()
        => Match(
            exception => $"Exception({exception.Message})",
            t => $"Success({t})");
}

public static class Exceptional {
    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public static Exceptional<R> Map<T, R>(this Exceptional<T> self, Func<T, R> f)
        => self.Match(
            ex => new Exceptional<R>(ex),
            r => f(r));

    /// <summary>
    /// Functor BiMap
    /// </summary>
    [Pure]
    public static Exceptional<R> BiMap<T, R>(this Exceptional<T> self, Func<Exception, Exception> ex, 
        Func<T, R> succ)
        => self.Match<Exceptional<R>>(l => ex(l), r => succ(r));

    /// <summary>
    /// Side-effecting Map
    /// </summary>
    [Pure]
    public static Exceptional<Unit> ForEach<T>(this Exceptional<T> self, Action<T> act)
        => Map(self, act.ToFunc());

    /// <summary>
    /// Monadic Bind
    /// </summary>
    [Pure]
    public static Exceptional<R> Bind<T, R>(this Exceptional<T> self, Func<T, Exceptional<R>> f)
        => self.Match(
            ex => new(ex), f);

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

    /// <summary>
    /// Convert an <see cref="Exceptional{T}"/> to a <see cref="Maybe{T}"/> 
    /// </summary>
    [Pure]
    public static Maybe<T> ToMaybe<T>(this Exceptional<T> self)
        => self.Match(_ => Nothing, Just);

    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public static Exceptional<R> Select<T, R>(this Exceptional<T> self, Func<T, R> f)
        => self.Map(f);

    /// <summary>
    /// Monadic bind with a projection function
    /// </summary>
    [Pure]
    public static Exceptional<RR> SelectMany<T, R, RR>(this Exceptional<T> self, Func<T, Exceptional<R>> bind,
        Func<T, R, RR> proj)
        => self.Bind(t => bind(t).Map(r => proj(t, r)));
}