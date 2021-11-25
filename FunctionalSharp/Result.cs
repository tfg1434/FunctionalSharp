using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp;

/// <summary>
/// Semantic version of Either&lt;Error, T&gt;
/// </summary>
public readonly struct Result<T> : IEquatable<Result<T>> {
    private readonly Error? _error;
    private readonly T? _value;
    private readonly bool _isSucc;

    /// <summary>
    /// Fail ctor
    /// </summary>
    /// <param name="error">Error value</param>
    /// <returns><see cref="Result{T}"/> in an Error state</returns>
    public Result(Error error) {
        _error = error;
        _value = default;
        _isSucc = false;
    }
    
    /// <summary>
    /// Success ctor
    /// </summary>
    /// <param name="value">Success value</param>
    /// <returns><see cref="Result{T}"/> in a Success state</returns>
    public Result(T value) {
        _error = default;
        _value = value;
        _isSucc = true;
    }
    
    /// <summary>
    /// Construct a success result
    /// </summary>
    /// <param name="value">Success value</param>
    /// <returns><see cref="Result{T}"/></returns>
    [Pure]
    public static Result<T> Of(T value) => new(value);

    /// <summary>
    /// Construct a fail result
    /// </summary>
    /// <param name="e">Error value</param>
    /// <returns><see cref="Result{T}"/></returns>
    [Pure]
    public static Result<T> Of(Error e) => new(e);

    [Pure]
    public static implicit operator Result<T>(Error e) => new(e);

    [Pure]
    public static implicit operator Result<T>(T t) => new(t);

    /// <summary>
    /// Match
    /// </summary>
    /// <param name="fail">Fail function</param>
    /// <param name="succ">Success function</param>
    /// <typeparam name="R">Return type</typeparam>
    [Pure]
    public R Match<R>(Func<Error, R> fail, Func<T, R> succ)
        => _isSucc ? succ(_value!) : fail(_error!);

    /// <summary>
    /// Side-effect <see cref="Match{R}"/>
    /// </summary>
    /// <param name="fail">Fail Action</param>
    /// <param name="succ">Success Action</param>
    public Unit Match(Action<Error> fail, Action<T> succ)
        => Match(fail.ToFunc(), succ.ToFunc());
    
    /// <summary>
    /// Convert to <see cref="IEnumerable{T}"/> if in Success state
    /// </summary>
    [Pure]
    public IEnumerable<T> AsEnumerable() {
        if (_isSucc) yield return _value!;
    }
    
    /// <summary>
    /// Convert a <see cref="Result{T}"/> to a <see cref="Maybe{T}"/> 
    /// </summary>
    [Pure]
    public Maybe<T> ToMaybe()
        => Match(_ => Nothing, Just);
    
    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="R">Resulting wrapped type</typeparam>
    [Pure]
    public Result<R> Map<R>(Func<T, R> f)
        => Match(
            fail => new Result<R>(fail),
            succ => f(succ));
    
    /// <summary>
    /// Functor BiMap
    /// </summary>
    [Pure]
    public Result<R> BiMap<R>(Func<T, R> succ, Func<Error, R> fail)
        => Match<Result<R>>(
            e => new(fail(e)),
            t => new(succ(t)));

    /// <summary>
    /// Side-effect functor Map
    /// </summary>
    /// <param name="act">Action function</param>
    public Result<Unit> ForEach(Action<T> act)
        => Map(act.ToFunc());

    /// <summary>
    /// Monadic Bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="R">Bind return wrapped type</typeparam>
    [Pure]
    public Result<R> Bind<R>(Func<T, Result<R>> f)
        => Match(fail => new(fail), f);

    /// <summary>
    /// Extract the value from the <see cref="Result{T}"/> with a default value
    /// </summary>
    /// <param name="value">Default value</param>
    [Pure]
    public T IfFail(in T value)
        => _isSucc ? _value! : value;
    
    /// <summary>
    /// Extract the value from the <see cref="Result{T}"/> with a lazy default value
    /// </summary>
    /// <param name="f">Lazy default value</param>
    [Pure]
    public T IfFail(Func<Error, T> f)
        => _isSucc ? _value! : f(_error!);

    /// <summary>
    /// Perform a side-effecting <see cref="Action"/> if the <see cref="Result{T}"/> is in a Fail state
    /// </summary>
    /// <param name="f">Side-effecting <see cref="Action"/></param>
    /// <returns>Unit</returns>
    public Unit IfFail(Action<Error> f) {
        if (!_isSucc) f(_error!);

        return Unit();
    }

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="R">Resulting wrapped type</typeparam>
    [Pure]
    public Result<R> Select<R>(Func<T, R> f)
        => Map(f);

    /// <summary>
    /// Monadic Bind with a projection
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="proj">Projection</param>
    /// <typeparam name="R">Bind return wrapped type</typeparam>
    /// <typeparam name="PR">Project return type</typeparam>
    [Pure]
    public Result<PR> SelectMany<R, PR>(Func<T, Result<R>> bind, Func<T, R, PR> proj)
        => Bind(t => bind(t).Map(r => proj(t, r)));

    #region Equality
    
    public bool Equals(Result<T> other)
        => _isSucc && other._isSucc && EqualityComparer<T>.Default.Equals(_value, other._value) ||
           !_isSucc && !other._isSucc && EqualityComparer<Error>.Default.Equals(_error, other._error);

    public override bool Equals(object? obj)
        => obj is Result<T> other && Equals(other);

    public override int GetHashCode()
        => _isSucc
            ? _value!.GetHashCode()
            : _error!.GetHashCode();
    
    [Pure]
    public static bool operator ==(Result<T> self, Result<T> other) 
        => self.Equals(other);

    [Pure]
    public static bool operator !=(Result<T> self, Result<T> other)
        => !(self == other);

    #endregion
    
    [Pure]
    public override string ToString()
        => Match(
            fail => $"Error({fail.Message})", 
            succ => $"Succ({succ})");
    
    internal Result<R> Cast<R>()
        => _isSucc
            ? F.Cast<R>(_value!)
                .Map(Result<R>.Of)
                .IfNothing(() => new(new Error(
                    $"Can't cast success value of type {nameof(T)} to {nameof(R)}")))
            : new(_error!);
}

public static class ResultExt {
    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Result<R> Apply<T, R>(this Result<Func<T, R>> self, Result<T> arg)
        => self.Match(
            fail => fail,
            f => arg.Match<Result<R>>(
                fail => fail,
                t => f(t)));

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Result<Func<T2, R>> Apply<T1, T2, R>(this Result<Func<T1, T2, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Result<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Result<Func<T1, T2, T3, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Result<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Result<Func<T1, T2, T3, T4, R>> self, 
        Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Result<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>(
        this Result<Func<T1, T2, T3, T4, T5, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Result<Func<T2, T3, T4, T5, T6, R>> Apply<T1, T2, T3, T4, T5, T6, R>(
        this Result<Func<T1, T2, T3, T4, T5, T6, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Applicative Apply. Applies if both are in Success state
    /// </summary>
    [Pure]
    public static Result<Func<T2, T3, T4, T5, T6, T7, R>> Apply<T1, T2, T3, T4, T5, T6, T7, R>(
        this Result<Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);
}