using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FunctionalSharp; 

public delegate (T Value, S State) Stateful<S, T>(S state);

public static partial class F {
    /// <summary>
    /// Construct a State monad
    /// </summary>
    /// <param name="value">Bound value</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Type of bound value</typeparam>
    /// <returns>State monad</returns>
    public static Stateful<S, T> State<S, T>(T value)
        => state => (value, state);
}

public static class StatefulExt {
    /// <summary>
    /// Run the state monad with the given state
    /// </summary>
    /// <param name="f">The state monad to run</param>
    /// <param name="state">The state to run with</param>
    /// <typeparam name="S">Type of state</typeparam>
    /// <typeparam name="T">Type of bound value</typeparam>
    /// <returns>Value</returns>
    public static T Run<S, T>(this Stateful<S, T> f, S state) 
        => f(state).Value;
    
    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="self">State monad</param>
    /// <param name="f">Bind function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    /// <typeparam name="R">Bind return type</typeparam>
    /// <returns>Bound state monad</returns>
    public static Stateful<S, R> Bind<S, T, R>(this Stateful<S, T> self, Func<T, Stateful<S, R>> f)
        => state => {
            (T t, S newState) = self(state);
            
            return f(t)(newState);
        };

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="self">State monad to map over</param>
    /// <param name="f">Map function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    /// <typeparam name="R">Map return type</typeparam>
    /// <returns>Mapped state monad</returns>
    [Pure]
    public static Stateful<S, R> Map<S, T, R>(this Stateful<S, T> self, Func<T, R> f)
        => state => {
            (T t, S newState) = self(state);
            
            return (f(t), newState);
        };

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="self">State monad to map over</param>
    /// <param name="f">Map function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    /// <typeparam name="R">Map return type</typeparam>
    /// <returns>Mapped state monad</returns>
    [Pure]
    public static Stateful<S, R> Select<S, T, R>(this Stateful<S, T> self, Func<T, R> f)
        => Map(self, f);

    /// <summary>
    /// Monadic bind with a projection
    /// </summary>
    /// <param name="self">State monad</param>
    /// <param name="bind">Bind function</param>
    /// <param name="proj">Projection function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    /// <typeparam name="R">Bind return type</typeparam>
    /// <typeparam name="PR">Project return type</typeparam>
    /// <returns>State monad bound and projected</returns>
    [Pure]
    public static Stateful<S, PR> SelectMany<S, T, R, PR>(this Stateful<S, T> self, Func<T, Stateful<S, R>> bind,
        Func<T, R, PR> proj)
        => self.Bind(t => bind(t).Map(r => proj(t, r)));

    /// <summary>
    /// Evaluate the state and return it in an <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <param name="self">State monad</param>
    /// <param name="state">State to evaluate with</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    /// <returns><see cref="IEnumerable{T}"/></returns>
    [Pure]
    public static IEnumerable<T> AsEnumerable<S, T>(this Stateful<S, T> self, S state) {
        yield return self(state).Value;
    }
}