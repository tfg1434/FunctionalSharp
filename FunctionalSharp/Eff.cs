using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public static partial class F {
    /// <summary>
    /// Construct an Eff that will succeed, exceptionally fail, or unexceptionally fail
    /// </summary>
    /// <param name="f"><see cref="Result{T}"/>-returning function</param>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <returns>Eff</returns>
    [Pure]
    public static Eff<T> EffMaybe<T>(Func<Result<T>> f) 
        => new(Thunk<T>.Of(f));

    /// <summary>
    /// Construct an Eff that will succeed or exceptionally fail
    /// </summary>
    /// <param name="f">Function to capture effect</param>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <returns>Eff</returns>
    [Pure]
    public static Eff<T> Eff<T>(Func<T> f)
        => new(Thunk<T>.Of(() => new(f())));

    /// <summary>
    /// Construct an Eff in a success state
    /// </summary>
    /// <param name="value">Success value</param>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <returns>Eff</returns>
    [Pure]
    public static Eff<T> EffSucc<T>(T value)
        => new(Thunk<T>.OfSucc(value));

    /// <summary>
    /// Construct an Eff in a fail state
    /// </summary>
    /// <param name="error">Error value</param>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <returns>Eff</returns>
    [Pure]
    public static Eff<T> EffFail<T>(Error error)
        => new(Thunk<T>.OfFail(error));
}

/// <summary>
/// Capture the side-effects of an operation without an environment
/// </summary>
/// <typeparam name="T">Wrapped type</typeparam>
public readonly struct Eff<T> {
    private readonly Thunk<T> _thunk;

    internal Eff(Thunk<T> thunk) 
        => _thunk = thunk;

    /// <summary>
    /// Run the Eff, use memoized value if available
    /// </summary>
    public Result<T> Run() 
        => _thunk.Value();
    
    /// <summary>
    /// Re-run the Eff, clearing the memoized value
    /// </summary>
    public Result<T> ReRun() 
        => _thunk.ReValue();

    /// <summary>
    /// Clone the Eff
    /// </summary>
    /// <returns>New Eff losing fail/success status</returns>
    [Pure]
    public Eff<T> Clone() 
        => new(_thunk.Clone());

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="R">Return type of map function</typeparam>
    /// <returns>Mapped Eff</returns>
    [Pure]
    public Eff<R> Map<R>(Func<T, R> f)
        => new(_thunk.Map(f));

    /// <summary>
    /// Functor BiMap
    /// </summary>
    /// <param name="succ">Success function</param>
    /// <param name="fail">Fail function</param>
    /// <typeparam name="R">Return type if in success state</typeparam>
    /// <returns>Mapped Eff</returns>
    [Pure]
    public Eff<R> BiMap<R>(Func<T, R> succ, Func<Error, Error> fail)
        => new(_thunk.BiMap(succ, fail));

    /// <summary>
    /// Lazily match the two states of the Eff
    /// </summary>
    /// <param name="succ">Success function</param>
    /// <param name="fail">Fail function</param>
    /// <typeparam name="R">Return type</typeparam>
    /// <returns>New Eff</returns>
    [Pure]
    public Eff<R> Match<R>(Func<T, R> succ, Func<Error, R> fail) {
        var @this = this;
        
        return Eff(() => {
            Result<T> res = @this.ReRun();

            return res.Match(fail, succ);
        });
    }

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="R">Wrapped return type</typeparam>
    /// <returns>Bound Eff</returns>
    [Pure]
    public Eff<R> Bind<R>(Func<T, Eff<R>> f) {
        var @this = this;
        
        return new(Thunk<R>.Of(() => {
            Result<T> res = @this.ReRun();

            return res.Match(Result<R>.Of, t => f(t).Run());
        }));
    }

    /// <summary>
    /// Use <paramref name="f"/> if the Eff is in a fail state
    /// </summary>
    /// <param name="f">Alternative lazy fail value taking an error</param>
    /// <returns>Lazy Eff with alternative fail value</returns>
    [Pure]
    public Eff<T> IfFail(Func<Error, T> f) {
        var @this = this;
        
        return EffMaybe(() => {
            var res = @this.ReRun();

            return res.Match(e => Result<T>.Of(f(e)), t => t);
        });
    }

    /// <summary>
    /// Use <paramref name="f"/> if the Eff is in a fail state
    /// </summary>
    /// <param name="f">Alternative lazy fail value</param>
    /// <returns>Lazy Eff with alternative fail value</returns>
    [Pure]
    public Eff<T> IfFail(Func<T> f) {
        var @this = this;
        
        return EffMaybe(() => {
            var res = @this.ReRun();
            
            return res.Match(_ => Result<T>.Of(f()), t => t);
        });
    }
    
    /// <summary>
    /// Use <paramref name="value"/> if the Eff is in a fail state
    /// </summary>
    /// <param name="value">Alternative fail value</param>
    /// <returns>Lazy Eff with alternative fail value</returns>
    [Pure]
    public Eff<T> IfFail(T value) {
        var @this = this;
        
        return EffMaybe(() => {
            var res = @this.ReRun();
            
            return res.Match(_ => Result<T>.Of(value), t => t);
        });
    }

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Map function</param>
    /// <typeparam name="R">Return type of map function</typeparam>
    /// <returns>Mapped Eff</returns>
    [Pure]
    public Eff<R> Select<R>(Func<T, R> f) => Map(f);
    
    /// <summary>
    /// Monadic Bind with a projection function
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="proj">Projection function</param>
    /// <typeparam name="R">Bind wrapped return type</typeparam>
    /// <typeparam name="PR">Project return type</typeparam>
    /// <returns>Bound and projected Eff</returns>
    [Pure]
    public Eff<PR> SelectMany<R, PR>(Func<T, Eff<R>> bind, Func<T, R, PR> proj)
        => Bind(t => bind(t).Map(r => proj(t, r)));
}
