using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public static partial class F {
    /// <summary>
    /// Construct an IO monad that will succeed, exceptionally fail, or unexceptionally fail
    /// </summary>
    /// <param name="f"><see cref="Result{T}"/>-returning function</param>
    /// <typeparam name="E">Type of environment</typeparam>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <returns>IO monad</returns>
    [Pure]
    public static IO<E, T> IOMaybe<E, T>(Func<E, Result<T>> f) where E : struct
        => new(Thunk<E, T>.Of(f));

    /// <summary>
    /// Construct an IO monad that will succeed or exceptionally fail
    /// </summary>
    /// <param name="f">Function to capture effect</param>
    /// <typeparam name="E">Type of environment</typeparam>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <returns>IO monad</returns>
    [Pure]
    public static IO<E, T> IO<E, T>(Func<E, T> f) where E : struct
        => new(Thunk<E, T>.Of(env => new(f(env))));

    /// <summary>
    /// Construct an IO monad in a success state
    /// </summary>
    /// <param name="value">Success value</param>
    /// <typeparam name="E">Type of environment</typeparam>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <returns>IO monad</returns>
    [Pure]
    public static IO<E, T> IOSucc<E, T>(T value) where E : struct
        => new(Thunk<E, T>.OfSucc(value));

    /// <summary>
    /// Construct an IO monad in a fail state
    /// </summary>
    /// <param name="error">Error value</param>
    /// <typeparam name="E">Type of environment</typeparam>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <returns>IO monad</returns>
    [Pure]
    public static IO<E, T> IOFail<E, T>(Error error) where E : struct
        => new(Thunk<E, T>.OfFail(error));
}

/// <summary>
/// The IO monad is <see cref="Eff{T}"/>, but with an environment typeclass
/// </summary>
/// <typeparam name="E">Environment typeclass</typeparam>
/// <typeparam name="T">Wrapped type</typeparam>
public readonly struct IO<E, T> where E : struct {
    private readonly Thunk<E, T> _thunk;

    internal IO(Thunk<E, T> thunk) => _thunk = thunk;

    [Pure]
    public static implicit operator IO<E, T>(Eff<T> eff) 
        => IOMaybe<E, T>(_ => eff.ReRun());

    /// <summary>
    /// Run the IO monad, use memoized value if available
    /// </summary>
    /// <param name="env">Environment to run in</param>
    public Result<T> Run(E env) 
        => _thunk.Value(env);

    /// <summary>
    /// Re-run the IO monad, clearing the memoized value
    /// </summary>
    /// <param name="env">Environment to run in</param>
    public Result<T> ReRun(E env) 
        => _thunk.ReValue(env);

    /// <summary>
    /// Clone the IO monad
    /// </summary>
    /// <returns>New IO monad losing fail/success status</returns>
    [Pure]
    public IO<E, T> Clone() 
        => new(_thunk.Clone());

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="R">Return type of map function</typeparam>
    /// <returns>Mapped IO monad</returns>
    [Pure]
    public IO<E, R> Map<R>(Func<T, R> f)
        => new(_thunk.Map(f));

    /// <summary>
    /// Functor BiMap
    /// </summary>
    /// <param name="succ">Success function</param>
    /// <param name="fail">Fail function</param>
    /// <typeparam name="R">Return type if in success state</typeparam>
    /// <returns>Mapped IO monad</returns>
    [Pure]
    public IO<E, R> BiMap<R>(Func<T, R> succ, Func<Error, Error> fail)
        => new(_thunk.BiMap(succ, fail));

    /// <summary>
    /// Lazily match the two states of the IO monad
    /// </summary>
    /// <param name="succ">Success function</param>
    /// <param name="fail">Fail function</param>
    /// <typeparam name="R">Return type</typeparam>
    /// <returns>New IO monad</returns>
    [Pure]
    public IO<E, R> Match<R>(Func<T, R> succ, Func<Error, R> fail) {
        var @this = this;

        return IO<E, R>(env => {
            Result<T> res = @this.Run(env);
            
            return res.Match(fail, succ);
        });
    }

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="R">Wrapped return type</typeparam>
    /// <returns>Bound IO monad</returns>
    [Pure]
    public IO<E, R> Bind<R>(Func<T, IO<E, R>> f) {
        var @this = this;

        return new(Thunk<E, R>.Of(env => {
            Result<T> res = @this.Run(env);

            return res.Match(Result<R>.Of, t => f(t).Run(env));
        }));
    }

    /// <summary>
    /// Use <paramref name="f"/> if the IO monad is in a fail state
    /// </summary>
    /// <param name="f">Alternative lazy fail value taking an error</param>
    /// <returns>Lazy IO monad with alternative fail value</returns>
    [Pure]
    public IO<E, T> IfFail(Func<Error, T> f) {
        var @this = this;
    
        return IOMaybe<E, T>(env => {
            var res = @this.ReRun(env);
    
            return res.Match(e => Result<T>.Of(f(e)), t => t);
        });
    }

    /// <summary>
    /// Use <paramref name="f"/> if the IO monad is in a fail state
    /// </summary>
    /// <param name="f">Alternative lazy fail value</param>
    /// <returns>Lazy IO monad with alternative fail value</returns>
    [Pure]
    public IO<E, T> IfFail(Func<T> f) {
        var @this = this;
    
        return IOMaybe<E, T>(env => {
            var res = @this.ReRun(env);
    
            return res.Match(_ => Result<T>.Of(f()), t => t);
        });
    }

    /// <summary>
    /// Use <paramref name="value"/> if the IO monad is in a fail state
    /// </summary>
    /// <param name="value">Alternative fail value</param>
    /// <returns>Lazy IO monad with alternative fail value</returns>
    [Pure]
    public IO<E, T> IfFail(T value) {
        var @this = this;
        
        return IOMaybe<E, T>(env => {
            var res = @this.ReRun(env);
    
            return res.Match(_ => Result<T>.Of(value), t => t);
        });
    }

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="R">Return type of map function</typeparam>
    /// <returns>Mapped IO monad</returns>
    [Pure]
    public IO<E, R> Select<R>(Func<T, R> f) => Map(f);
    
    /// <summary>
    /// Monadic Bind with a projection function
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="proj">Projection function</param>
    /// <typeparam name="R">Bind wrapped return type</typeparam>
    /// <typeparam name="PR">Project return type</typeparam>
    /// <returns>Bound and projected IO monad</returns>
    [Pure]
    public IO<E, PR> SelectMany<R, PR>(Func<T, IO<E, R>> bind, Func<T, R, PR> proj)
        => Bind(t => bind(t).Map(r => proj(t, r)));
}
