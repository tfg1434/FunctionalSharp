using System;

namespace FPLibrary; 

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

    public static Thunk<E, T> Of(Func<E, Result<T>> f) => new(f);

    public static Thunk<E, T> OfSucc(T value) => new(value);
    
    public static Thunk<E, T> OfFail(Error error) => new(Thunk.Fail, error);
    
    public static Thunk<E, T> OfCancelled() => new(Thunk.Cancelled, new CancelledError());

    public Result<T> Value(E env) => Eval(env);

    public Result<T> ReValue(E env) {
        if (_f is not null)
            _state = Thunk.NotEvaluated;

        return Eval(env);
    }

    public Thunk<E, T> Clone()
        => _f is null
            ? new Thunk<E, T>(_state, _error!)
            : new(_f);

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
    
    public override string ToString() 
        => _state switch {
            Thunk.NotEvaluated => "NotEvaluated",
            Thunk.Succ => $"Succ({_value})",
            Thunk.Cancelled => "Cancelled",
            Thunk.Fail => $"Fail({_error})",
            _ => throw new InvalidOperationException("wtf"),
        };
}
