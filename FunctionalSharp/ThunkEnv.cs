using System;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

/// <summary>
/// Lazily evaluate a function with an environment, and memoize the value
/// </summary>
/// <typeparam name="E">Environment typeclass</typeparam>
/// <typeparam name="T">Wrapped type</typeparam>
public class Thunk<E, T> {
    private readonly Func<E, Result<T>>? _f;
    private Error? _error;
    private T? _value;
    private int _state;

    private Thunk(T value) {
        _state = Thunk.Succ;
        _value = value;
    }
    
    private Thunk(int state, Error e) {
        _state = state;
        _error = e;
    }
    
    private Thunk(Func<E, Result<T>> f) => _f = f;

    /// <summary>
    /// Create a lazy thunk
    /// </summary>
    /// <param name="f">Lazy computation</param>
    /// <returns>Lazy thunk</returns>
    [Pure]
    public static Thunk<E, T> Of(Func<E, Result<T>> f) => new(f);

    /// <summary>
    /// Create a success thunk
    /// </summary>
    /// <param name="value">Success value</param>
    /// <returns>Success thunk</returns>
    [Pure]
    public static Thunk<E, T> OfSucc(T value) => new(value);
    
    /// <summary>
    /// Construct a failure thunk
    /// </summary>
    /// <param name="error">Error value</param>
    /// <returns>Failure thunk</returns>
    [Pure]
    public static Thunk<E, T> OfFail(Error error) => new(Thunk.Fail, error);
    
    /// <summary>
    /// Construct a failure thunk
    /// </summary>
    /// <returns>Failure thunk</returns>
    [Pure]
    public static Thunk<E, T> OfCancelled() => new(Thunk.Cancelled, new CancelledError());

    /// <summary>
    /// Evaluate the thunk, use memoized value if available
    /// </summary>
    /// <param name="env">Environment to eval in</param>
    /// <returns>Value wrapped in <see cref="Result{T}"/> monad</returns>
    public Result<T> Value(E env) => Eval(env);

    /// <summary>
    /// Evaluate the thunk, refresh memoized value
    /// </summary>
    /// <param name="env">Environment to eval in</param>
    /// <returns>Value wrapped in <see cref="Result{T}"/> monad</returns>
    [Pure]
    public Result<T> ReValue(E env) {
        if (_f is not null)
            _state = Thunk.NotEvaluated;

        return Eval(env);
    }

    /// <summary>
    /// Clone and return the thunk
    /// </summary>
    /// <returns>Cloned thunk</returns>
    /// <remarks>If a failed or cancelled thunk, cloned thunk will be the same. Evaluated thunk will become
    /// non-evaluated; failed thunk will thus lose fail status.</remarks>
    [Pure]
    public Thunk<E, T> Clone()
        => _f is null
            ? new Thunk<E, T>(_state, _error!)
            : new(_f);

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Function to map</param>
    /// <typeparam name="R">Resulting type</typeparam>
    /// <returns>Mapped thunk</returns>
    /// <exception cref="InvalidOperationException">Should never happen</exception>
    [Pure]
    public Thunk<E, R> Map<R>(Func<T, R> f) {
        try {
            return _state switch {
                Thunk.Succ => Thunk<E, R>.OfSucc(f(_value!)),
                Thunk.NotEvaluated => Thunk<E, R>.Of(env => {
                    Result<T> res = Eval(env);

                    if (res.IsSucc) return f(res.Value!);

                    return res.Cast<R>();
                }),
                Thunk.Cancelled => Thunk<E, R>.OfCancelled(),
                Thunk.Fail => Thunk<E, R>.OfFail(_error!),
                _ => throw new InvalidOperationException("wtf"),
            };
        } catch (Exception e) {
            return Thunk<E, R>.OfFail(new Error(e));
        }
    }

    /// <summary>
    /// Functor BiMap
    /// </summary>
    /// <param name="succ">Success function</param>
    /// <param name="fail">Fail function</param>
    /// <typeparam name="R">Resulting type</typeparam>
    /// <returns>Mapped thunk</returns>
    /// <exception cref="InvalidOperationException">Should never happen</exception>
    [Pure]
    public Thunk<E, R> BiMap<R>(Func<T, R> succ, Func<Error, Error> fail) {
        try {
            return _state switch {
                Thunk.Succ => Thunk<E, R>.OfSucc(succ(_value!)),
                Thunk.NotEvaluated => Thunk<E, R>.Of(env => {
                    Result<T> res = Eval(env);

                    return res.IsSucc
                        ? new(succ(res.Value!))
                        : new(fail(res.Error!));
                }),
                Thunk.Cancelled => Thunk<E, R>.OfFail(fail(new CancelledError())),
                Thunk.Fail => Thunk<E, R>.OfFail(fail(_error!)),
                _ => throw new InvalidOperationException("wtf"),
            };
        } catch (Exception e) {
            return Thunk<E, R>.OfFail(fail(new(e)));
        }
    }

    private Result<T> Eval(E env) {
        if (_state == Thunk.NotEvaluated) {
            try {
                Result<T> res = _f!(env);

                if (res.IsFail) {
                    _error = res.Error;
                    _state = Thunk.Fail;

                    return res;
                }

                _value = res.Value;
                _state = Thunk.Succ;

                return res;
                
            } catch (Exception e) {
                _error = new(e);
                _state = Thunk.Fail;

                return _error;
            }
        }

        return _state switch {
            Thunk.Succ => _value!,
            Thunk.Cancelled => new CancelledError(),
            Thunk.Fail => _error!,
            _ => throw new InvalidOperationException("wtf"),
        };
    }
    
    [Pure]
    public override string ToString() 
        => _state switch {
            Thunk.NotEvaluated => "NotEvaluated",
            Thunk.Succ => $"Succ({_value})",
            Thunk.Cancelled => "Cancelled",
            Thunk.Fail => $"Fail({_error})",
            _ => throw new InvalidOperationException("wtf"),
        };
}
