using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp;

using static F;

/// <summary>
/// <see cref="IEnumerable{T}"/> extensions
/// </summary>
public static class EnumberableExt {
    /// <summary>
    /// Functor Map
    /// </summary>
    [Pure]
    public static IEnumerable<R> Map<T, R>(this IEnumerable<T> enumerable, Func<T, R> f)
        => enumerable.Select(f);

    /// <summary>
    /// Side-effecting Map
    /// </summary>
    /// <returns>Unit</returns>
    public static Unit ForEach<T>(this IEnumerable<T> ts, Action<T> act) {
        foreach (T t in ts) act(t);

        return Unit();
    }

    /// <summary>
    /// Monadic Bind
    /// </summary>
    [Pure]
    public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> ts, Func<T, IEnumerable<R>> f)
        => ts.SelectMany(f);
    
    /// <summary>
    /// Monadic bind, but give IEnumerable&lt;R&gt; instead of IEnumerable&lt;Maybe&lt;R&gt;&gt;
    /// </summary>
    [Pure]
    public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> ts, Func<T, Maybe<R>> f)
        => ts.Bind(t => f(t).AsEnumerable());

    /// <summary>
    /// Equivalent to Enumerable.FirstOrDefault, except returns <see cref="Nothing"/> if not found
    /// </summary>
    [Pure]
    public static Maybe<T> First<T>(this IEnumerable<T> ts, Func<T, bool> p) {
        foreach (T t in ts)
            if (p(t))
                return t;

        return Nothing;
    }

    /// <summary>
    /// Flatten. Equivalent to <c>.Bind(ident)</c>
    /// </summary>
    [Pure]
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        => enumerable.Bind(x => x);
}