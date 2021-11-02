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
    
}

public static class Thunk {
    public const int NotEvaluated = 0;
    public const int Evaluating = 1;
    public const int Succ = 2;
    public const int Fail = 4;
    public const int Cancelled = 8;
    public const int IsEvaluated = Succ | Fail | Cancelled;
}