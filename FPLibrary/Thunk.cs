﻿using System;

namespace FPLibrary;

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

    private Thunk(Func<Result<T>> f) 
        => _f = f ?? throw new ArgumentNullException(nameof(f));

    public static Thunk<T> OfSucc(Func<Result<T>> f) => new(f);

    public static Thunk<T> OfSucc(T value) => new(value);

    public static Thunk<T> OfFail(Error e) => new(Thunk.Fail, e);

    public static Thunk<T> OfCancelled() => new(Thunk.Cancelled, CancelledError.Of());

    public Result<T> Value() => Eval();

    public Result<T> ReValue() {
        if (_f is not null)
            _state = Thunk.NotEvaluated;

        return Eval();
    }
    
    public Thunk<T> Clone()
        => _f is null
            ? new(_state, _error!)
            : new(_f);

    public Thunk<R> Map<R>(Func<T, R> f) {
        try {
            switch (_state) {
                case Thunk.Succ:
                    return Thunk<R>.OfSucc(f(_value!));
                case Thunk.NotEvaluated:
                    return Thunk<R>.OfSucc(() => {
                        Result<T> res = Eval();

                        if (res.IsSucc)
                            return f(res.Value);
                        
                        return res.
                    });
            }
        }
    }
    
    private Result<T> Eval() {
        if (_state == Thunk.NotEvaluated) {
            try {
                var res = _f!();

                if (res.IsFail) {
                    _error = res.Error;
                    _state = Thunk.Fail;

                    return res;
                }

                _value = res.Value;
                _state = Thunk.Succ;

                return res;
                
            } catch (Exception e) {
                _error = Error.Of(e);

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