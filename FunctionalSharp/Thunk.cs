using System;
using System.Diagnostics.Contracts;

namespace FunctionalSharp;

/// <summary>
/// Lazily evaluate a function and memoize the value
/// </summary>
/// <typeparam name="T">Wrapped type</typeparam>
public class Thunk<T> {
    private readonly Func<Result<T>>? _f;
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

    private Thunk(Func<Result<T>> f) => _f = f;

    /// <summary>
    /// Create a lazy thunk
    /// </summary>
    /// <param name="f">Lazy computation</param>
    /// <returns>Lazy thunk</returns>
    [Pure]
    public static Thunk<T> Of(Func<Result<T>> f) => new(f);

    /// <summary>
    /// Create a success thunk
    /// </summary>
    /// <param name="value">Success value</param>
    /// <returns>Success thunk</returns>
    [Pure]
    public static Thunk<T> OfSucc(T value) => new(value);

    /// <summary>
    /// Construct a failure thunk
    /// </summary>
    /// <param name="error">Error value</param>
    /// <returns>Failure thunk</returns>
    [Pure]
    public static Thunk<T> OfFail(Error error) => new(Thunk.Fail, error);

    /// <summary>
    /// Construct a cancelled thunk
    /// </summary>
    /// <returns>Cancelled thunk</returns>
    [Pure]
    public static Thunk<T> OfCancelled() => new(Thunk.Cancelled, new CancelledError());

    /// <summary>
    /// Evaluate the thunk, use memoized value if available
    /// </summary>
    /// <returns>Value wrapped in <see cref="Result{T}"/> monad</returns>
    [Pure]
    public Result<T> Value() => Eval();

    /// <summary>
    /// Evaluate the thunk, refresh memoized value
    /// </summary>
    /// <returns>Value wrapped in <see cref="Result{T}"/> monad</returns>
    [Pure]
    public Result<T> ReValue() {
        if (_f is not null)
            _state = Thunk.NotEvaluated;

        return Eval();
    }
    
    /// <summary>
    /// Clone and return the thunk
    /// </summary>
    /// <returns>Cloned thunk</returns>
    /// <remarks>If a failed or cancelled thunk, cloned thunk will be the same. Evaluated thunk will become
    /// non-evaluated; failed thunk will thus lose fail status.</remarks>
    [Pure]
    public Thunk<T> Clone()
        => _f is null
            ? new(_state, _error!)
            : new(_f);

    /// <summary>
    /// Functor Map
    /// </summary>
    /// <param name="f">Function to map</param>
    /// <typeparam name="R">Resulting type</typeparam>
    /// <returns>Mapped thunk</returns>
    /// <exception cref="InvalidOperationException">Should never happen</exception>
    [Pure]
    public Thunk<R> Map<R>(Func<T, R> f) {
        try {
            return _state switch {
                Thunk.Succ => Thunk<R>.OfSucc(f(_value!)),
                Thunk.NotEvaluated => Thunk<R>.Of(() => {
                    Result<T> res = Eval();
                    
                    if (res.IsSucc) return f(res.Value!);
                    
                    return res.Cast<R>();
                }),
                Thunk.Cancelled => Thunk<R>.OfCancelled(),
                Thunk.Fail => Thunk<R>.OfFail(_error!),
                _ => throw new InvalidOperationException("wtf"),
            };
        } catch (Exception e) {
            return Thunk<R>.OfFail(new Error(e));
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
    public Thunk<R> BiMap<R>(Func<T, R> succ, Func<Error, Error> fail) {
        try {
            return _state switch {
                Thunk.Succ => Thunk<R>.OfSucc(succ(_value!)),
                Thunk.NotEvaluated => Thunk<R>.Of(() => {
                    Result<T> res = Eval();

                    return res.IsSucc
                        ? new(succ(res.Value!))
                        : new(fail(res.Error!));
                }),
                Thunk.Cancelled => Thunk<R>.OfFail(fail(new CancelledError())),
                Thunk.Fail => Thunk<R>.OfFail(fail(_error!)),
                _ => throw new InvalidOperationException("wtf"),
            };
        } catch (Exception e) {
            return Thunk<R>.OfFail(fail(new(e)));
        }
    }
    
    private Result<T> Eval() {
        if (_state == Thunk.NotEvaluated) {
            try {
                Result<T> res = _f!();

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

public static class Thunk {
    public const int NotEvaluated = 0;
    public const int Succ = 1;
    public const int Fail = 2;
    public const int Cancelled = 4;
    public const int IsEvaluated = Succ | Fail | Cancelled;
}