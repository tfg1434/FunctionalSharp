using System;

namespace FPLibrary;

public class Thunk<T> {
    private readonly Func<Result<T>> _f;
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

    public static Thunk<T> OfSucc(Func<Result<T>> f) => new(f);

    public static Thunk<T> OfSucc(T value) => new(value);

    public static Thunk<T> OfFail(Error e) => new(Thunk.Fail, e);

    public static Thunk<T> OfCancelled() => new(Thunk.Cancelled, new CancelledError());

    private Result<T> Eval() {
        if (_state == Thunk.NotEvaluated) {
            try {
                var res = _f();

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
                
            }
        }
    }
    
    /*
    Interlocked.CompareExchange(ref state, Thunk.Evaluating, Thunk.NotEvaluated) == Thunk.NotEvaluated
    
    if (state == notEvaluated)
        do shit
        
    if (state == not evaluated) then state = evaluating
     */
    
}

public static class Thunk {
    public const int NotEvaluated = 0;
    public const int Evaluating = 1;
    public const int Succ = 2;
    public const int Fail = 4;
    public const int Cancelled = 8;
    public const int IsEvaluated = Succ | Fail | Cancelled;
}