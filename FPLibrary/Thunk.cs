using System;

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

    public static Thunk<T> OfCancelled() => new(Thunk.Cancelled, new CancelledError());

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
                            return f(res.Value!);

                        return res.Cast<R>();
                    });
                case Thunk.Cancelled:
                    return Thunk<R>.OfCancelled();
                case Thunk.Fail:
                    return Thunk<R>.OfFail(_error!);
                default:
                    throw new InvalidOperationException("wtf");
            }
        } catch (Exception e) {
            return Thunk<R>.OfFail(new Error(e));
        }
    }

    public Thunk<R> BiMap<R>(Func<T, R> succ, Func<Error, Error> fail) {
        try {
            return _state switch {
                Thunk.Succ => Thunk<R>.OfSucc(succ(_value!)),
                Thunk.NotEvaluated => Thunk<R>.OfSucc(() => {
                    Result<T> res = Eval();

                    return res.IsSucc
                        ? Result<R>.Succ(succ(res.Value!))
                        : Result<R>.Fail(fail(res.Error!));
                }),
                Thunk.Cancelled => Thunk<R>.OfFail(fail(new CancelledError())),
            };
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
                _error = new Error(e);

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