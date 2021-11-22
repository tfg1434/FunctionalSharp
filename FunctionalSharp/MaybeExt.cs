using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

/// <summary>
/// Extension methods for the Maybe type
/// </summary>
public static class MaybeExt {
    /// <summary>
    /// Map for Maybe in Nothing state
    /// </summary>
    /// <remarks>Does absolutely nothing</remarks>
    [Pure]
    public static Maybe<R> Map<T, R>(this NothingType _, Func<T, R> __) => Nothing;
    
    /// <summary>
    /// Apply a Maybe&lt;T&gt; to a Maybe&lt;Func&lt;T, R&gt;&gt; if both are in Just state
    /// </summary>
    /// <param name="self">Function wrapped in <see cref="Maybe{T}"/></param>
    /// <param name="arg">Value to Apply, wrapped in <see cref="Maybe{T}"/></param>
    /// <typeparam name="T">Type of Maybe to apply function to</typeparam>
    /// <typeparam name="R">Return type of function</typeparam>
    /// <returns>Maybe with function applied</returns>
    [Pure]
    public static Maybe<R> Apply<T, R>(this Maybe<Func<T, R>> self, Maybe<T> arg)
        => self.Match(
            () => Nothing,
            f => arg.Match(
                () => Nothing,
                val => Just(f(val))));

    /// <summary>
    /// Apply a Maybe&lt;T1&gt; to a Maybe&lt;Func&lt;T1, T2, R&gt;&gt; if both are in Just state
    /// </summary>
    /// <param name="self">Function wrapped in <see cref="Maybe{T}"/></param>
    /// <param name="arg">Value to Apply, wrapped in <see cref="Maybe{T}"/></param>
    /// <typeparam name="T1">First type of Maybe to apply function to</typeparam>
    /// <typeparam name="T2">Second type of Maybe to apply function to</typeparam>
    /// <typeparam name="R">Return type of function</typeparam>
    /// <returns>Maybe with function applied</returns>
    [Pure]
    public static Maybe<Func<T2, R>> Apply<T1, T2, R>(this Maybe<Func<T1, T2, R>> self, Maybe<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Apply a Maybe&lt;T1&gt; to a Maybe&lt;Func&lt;T1, T2, R&gt;&gt; if both are in Just state
    /// </summary>
    /// <param name="self">Function wrapped in <see cref="Maybe{T}"/></param>
    /// <param name="arg">Value to Apply, wrapped in <see cref="Maybe{T}"/></param>
    /// <typeparam name="T1">First type of Maybe to apply function to</typeparam>
    /// <typeparam name="T2">Second type of Maybe to apply function to</typeparam>
    /// <typeparam name="T3">Third type of Maybe to apply function to</typeparam>
    /// <typeparam name="R">Return type of function</typeparam>
    /// <returns>Maybe with function applied</returns>
    [Pure]
    public static Maybe<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Maybe<Func<T1, T2, T3, R>> self, Maybe<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Apply a Maybe&lt;T1&gt; to a Maybe&lt;Func&lt;T1, T2, R&gt;&gt; if both are in Just state
    /// </summary>
    /// <param name="self">Function wrapped in <see cref="Maybe{T}"/></param>
    /// <param name="arg">Value to Apply, wrapped in <see cref="Maybe{T}"/></param>
    /// <typeparam name="T1">First type of Maybe to apply function to</typeparam>
    /// <typeparam name="T2">Second type of Maybe to apply function to</typeparam>
    /// <typeparam name="T3">Third type of Maybe to apply function to</typeparam>
    /// <typeparam name="T4">Fourth type of Maybe to apply function to</typeparam>
    /// <typeparam name="R">Return type of function</typeparam>
    /// <returns>Maybe with function applied</returns>
    [Pure]
    public static Maybe<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Maybe<Func<T1, T2, T3, T4, R>> self,
        Maybe<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Apply a Maybe&lt;T1&gt; to a Maybe&lt;Func&lt;T1, T2, R&gt;&gt; if both are in Just state
    /// </summary>
    /// <param name="self">Function wrapped in <see cref="Maybe{T}"/></param>
    /// <param name="arg">Value to Apply, wrapped in <see cref="Maybe{T}"/></param>
    /// <typeparam name="T1">First type of Maybe to apply function to</typeparam>
    /// <typeparam name="T2">Second type of Maybe to apply function to</typeparam>
    /// <typeparam name="T3">Third type of Maybe to apply function to</typeparam>
    /// <typeparam name="T4">Fourth type of Maybe to apply function to</typeparam>
    /// <typeparam name="T5">Fifth type of Maybe to apply function to</typeparam>
    /// <typeparam name="R">Return type of function</typeparam>
    /// <returns>Maybe with function applied</returns>
    [Pure]
    public static Maybe<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>(
        this Maybe<Func<T1, T2, T3, T4, T5, R>> self, Maybe<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Apply a Maybe&lt;T1&gt; to a Maybe&lt;Func&lt;T1, T2, R&gt;&gt; if both are in Just state
    /// </summary>
    /// <param name="self">Function wrapped in <see cref="Maybe{T}"/></param>
    /// <param name="arg">Value to Apply, wrapped in <see cref="Maybe{T}"/></param>
    /// <typeparam name="T1">First type of Maybe to apply function to</typeparam>
    /// <typeparam name="T2">Second type of Maybe to apply function to</typeparam>
    /// <typeparam name="T3">Third type of Maybe to apply function to</typeparam>
    /// <typeparam name="T4">Fourth type of Maybe to apply function to</typeparam>
    /// <typeparam name="T5">Fifth type of Maybe to apply function to</typeparam>
    /// <typeparam name="T6">Sixth type of Maybe to apply function to</typeparam>
    /// <typeparam name="R">Return type of function</typeparam>
    /// <returns>Maybe with function applied</returns>
    [Pure]
    public static Maybe<Func<T2, T3, T4, T5, T6, R>> Apply<T1, T2, T3, T4, T5, T6, R>(
        this Maybe<Func<T1, T2, T3, T4, T5, T6, R>> self, Maybe<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    /// <summary>
    /// Apply a Maybe&lt;T1&gt; to a Maybe&lt;Func&lt;T1, T2, R&gt;&gt; if both are in Just state
    /// </summary>
    /// <param name="self">Function wrapped in <see cref="Maybe{T}"/></param>
    /// <param name="arg">Value to Apply, wrapped in <see cref="Maybe{T}"/></param>
    /// <typeparam name="T1">First type of Maybe to apply function to</typeparam>
    /// <typeparam name="T2">Second type of Maybe to apply function to</typeparam>
    /// <typeparam name="T3">Third type of Maybe to apply function to</typeparam>
    /// <typeparam name="T4">Fourth type of Maybe to apply function to</typeparam>
    /// <typeparam name="T5">Fifth type of Maybe to apply function to</typeparam>
    /// <typeparam name="T6">Sixth type of Maybe to apply function to</typeparam>
    /// <typeparam name="T7">Seventh type of Maybe to apply function to</typeparam>
    /// <typeparam name="R">Return type of function</typeparam>
    /// <returns>Maybe with function applied</returns>
    [Pure]
    public static Maybe<Func<T2, T3, T4, T5, T6, T7, R>> Apply<T1, T2, T3, T4, T5, T6, T7, R>(
        this Maybe<Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Maybe<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);
}

public readonly partial struct Maybe<T> {
    /// <summary>
    /// Return wrapped value of Maybe; if Nothing, throw <see cref="InvalidOperationException"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">If Maybe is in Nothing state</exception>
    [Pure]
    public T NotNothing() 
        => Match(() => throw new InvalidOperationException(), t => t);
        
    /// <summary>
    /// Return wrapped value of Maybe; if Nothing, use <paramref name="val"/>
    /// </summary>
    [Pure]
    public T IfNothing(T val) {
        if (val is null) throw new ArgumentNullException(nameof(val));

        return IsJust ? _value! : val;
    }

    /// <summary>
    /// Return wrapped value of Maybe; if Nothing, use lazy <paramref name="f"/>
    /// </summary>
    [Pure]
    public T IfNothing(Func<T> f) {
        if (f is null) throw new ArgumentNullException(nameof(f));

        return IsJust ? _value! : f() ?? throw new ArgumentException("Callback returned null", nameof(f));
    }

    /// <summary>
    /// If Maybe is in Nothing state, run <paramref name="f"/>
    /// </summary>
    [Pure]
    public Unit IfNothing(Action f) {
        if (IsNothing) f();

        return Unit();
    }  
        
    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="R">Map function return type</typeparam>
    /// <returns>Mapped Maybe monad</returns>
    [Pure]
    public Maybe<R> Map<R>(Func<T, R> f)
        => Match(() => Nothing, t => Just(f(t)));

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="T2">Second type of Map function</typeparam>
    /// <typeparam name="R">Map function return type</typeparam>
    /// <returns>Mapped Maybe monad</returns>
    [Pure]
    public Maybe<Func<T2, R>> Map<T2, R>(Func<T, T2, R> f)
        => Map(f.CurryFirst());

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="T2">Second type of Map function</typeparam>
    /// <typeparam name="T3">Third type of Map function</typeparam>
    /// <typeparam name="R">Map function return type</typeparam>
    /// <returns>Mapped Maybe monad</returns>
    [Pure]
    public Maybe<Func<T2, T3, R>> Map<T2, T3, R>(Func<T, T2, T3, R> f)
        => Map(f.CurryFirst());

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="T2">Second type of Map function</typeparam>
    /// <typeparam name="T3">Third type of Map function</typeparam>
    /// <typeparam name="T4">Fourth type of Map function</typeparam>
    /// <typeparam name="R">Map function return type</typeparam>
    /// <returns>Mapped Maybe monad</returns>
    [Pure]
    public Maybe<Func<T2, T3, T4, R>> Map<T2, T3, T4, R>(Func<T, T2, T3, T4, R> f)
        => Map(f.CurryFirst());

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="T2">Second type of Map function</typeparam>
    /// <typeparam name="T3">Third type of Map function</typeparam>
    /// <typeparam name="T4">Fourth type of Map function</typeparam>
    /// <typeparam name="T5">Fifth type of Map function</typeparam>
    /// <typeparam name="R">Map function return type</typeparam>
    /// <returns>Mapped Maybe monad</returns>
    [Pure]
    public Maybe<Func<T2, T3, T4, T5, R>> Map<T2, T3, T4, T5, R>(
        Func<T, T2, T3, T4, T5, R> f)
        => Map(f.CurryFirst());

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="T2">Second type of Map function</typeparam>
    /// <typeparam name="T3">Third type of Map function</typeparam>
    /// <typeparam name="T4">Fourth type of Map function</typeparam>
    /// <typeparam name="T5">Fifth type of Map function</typeparam>
    /// <typeparam name="T6">Sixth type of Map function</typeparam>
    /// <typeparam name="R">Map function return type</typeparam>
    /// <returns>Mapped Maybe monad</returns>
    [Pure]
    public Maybe<Func<T2, T3, T4, T5, T6, R>> Map<T2, T3, T4, T5, T6, R>(
        Func<T, T2, T3, T4, T5, T6, R> f)
        => Map(f.CurryFirst());

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="T2">Second type of Map function</typeparam>
    /// <typeparam name="T3">Third type of Map function</typeparam>
    /// <typeparam name="T4">Fourth type of Map function</typeparam>
    /// <typeparam name="T5">Fifth type of Map function</typeparam>
    /// <typeparam name="T6">Sixth type of Map function</typeparam>
    /// <typeparam name="T7">Seventh type of Map function</typeparam>
    /// <typeparam name="R">Map function return type</typeparam>
    /// <returns>Mapped Maybe monad</returns>
    [Pure]
    public Maybe<Func<T2, T3, T4, T5, T6, T7, R>> Map<T2, T3, T4, T5, T6, T7, R>(
        Func<T, T2, T3, T4, T5, T6, T7, R> f)
        => Map(f.CurryFirst());
        
    /// <summary>
    /// Side-effect Map
    /// </summary>
    /// <param name="act">Side-effecting Action</param>
    /// <returns>Unit wrapped in <see cref="Maybe{T}"/></returns>
    public Maybe<Unit> ForEach(Action<T> act)
        => Map(act.ToFunc());

    /// <summary>
    /// Monadic Bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="R">Type of wrapped return value</typeparam>
    [Pure]
    public Maybe<R> Bind<R>(Func<T, Maybe<R>> f)
        => Match(() => Nothing, f);

    /// <summary>
    /// Monadic Bind, but give <see cref="IEnumerable{R}"/> instead of <c>Maybe&lt;IEnumerable&lt;R&gt;&gt;</c>
    /// </summary>
    /// <param name="f"></param>
    /// <typeparam name="R"></typeparam>
    /// <returns></returns>
    [Pure]
    public IEnumerable<R> Bind<R>(Func<T, IEnumerable<R>> f)
        => AsEnumerable().Bind(f);
        
    /// <summary>
    /// Convert this <see cref="Maybe{T}"/> to <see cref="Either{L, R}"/>
    /// </summary>
    /// <param name="f">Lazy left value</param>
    /// <typeparam name="L">Type of left value</typeparam>
    /// <returns>Either in fail or success state, depending on state of Maybe</returns>
    [Pure]
    public Either<L, T> ToEither<L>(Func<L> f)
        => Match<Either<L, T>>(() => f(), r => r);

    /// <summary>
    /// Convert this <see cref="Maybe{T}"/> to <see cref="Either{L, R}"/>
    /// </summary>
    /// <param name="defaultLeft">Left value</param>
    /// <typeparam name="L">Type of left value</typeparam>
    /// <returns>Either in fail or success state, depending on state of Maybe</returns>
    [Pure]
    public Either<L, T> ToEither<L>(L defaultLeft)
        => Match<Either<L, T>>(() => defaultLeft, r => r);

    /// <summary>
    /// Side-effecting Match
    /// </summary>
    /// <param name="nothing">Action to run if in Nothing state</param>
    /// <param name="just">Action to run if in Just state</param>
    /// <returns>Unit</returns>
    [Pure]
    public Unit Match(Action nothing, Action<T> just)
        => Match(nothing.ToFunc(), just.ToFunc());

    /// <summary>
    /// Get the value of the Maybe, or use the default value
    /// </summary>
    /// <param name="defaultVal">Default value if in Nothing state</param>
    /// <returns>Value of Maybe, or <paramref name="defaultVal"/></returns>
    [Pure]
    public T GetOr(T defaultVal)
        => Match(() => defaultVal, t => t);

    /// <summary>
    /// Get the value of the Maybe, or use the lazy default value
    /// </summary>
    /// <param name="f">Lazy default value if in Nothing state</param>
    /// <returns>Value of Maybe, or <paramref name="f"/></returns>
    [Pure]
    public T GetOr(Func<T> f)
        => Match(f, t => t);

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="R">Map function return type</typeparam>
    /// <returns>Mapped Maybe monad</returns>
    [Pure]
    public Maybe<R> Select<R>(Func<T, R> f) => Map(f);

    /// <summary>
    /// Functor Bind with a projection
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="proj">Project function</param>
    /// <typeparam name="R">Bind function wrapped return type</typeparam>
    /// <typeparam name="RR">Project function return type</typeparam>
    /// <returns>Bound and projected Maybe monad</returns>
    [Pure]
    public Maybe<RR> SelectMany<R, RR>(Func<T, Maybe<R>> bind, Func<T, R, RR> proj)
        => Match(
            () => Nothing,
            t => bind(t).Match(
                () => Nothing,
                r => Just(proj(t, r))));

    /// <summary>
    /// Filter the Maybe
    /// </summary>
    /// <param name="p">Predicate function</param>
    /// <returns>Filtered Maybe monad</returns>
    [Pure]
    public Maybe<T> Where(Func<T, bool> p) {
        var self = this;
            
        return Match(
            () => Nothing,
            t => p(t) ? self : Nothing);
    }
}