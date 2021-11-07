using System;
using static FPLibrary.F;
using Unit = System.ValueTuple;

namespace FPLibrary;

//TODO: refactor these discriminated unions
public readonly struct Result<T> {
    private readonly Error? _error;
    private readonly T? _value;

    internal Result(Error error) {
        _error = error;
        _value = default;
        IsSucc = false;
    }

    internal Result(T value) {
        _error = default;
        _value = value;
        IsSucc = true;
    }

    public static Result<T> Succ(T value) => new(value);

    public static Result<T> Fail(Error e) => new(e);

    public static implicit operator Result<T>(Error e) => new(e);

    public static implicit operator Result<T>(T t) => new(t);

    public bool IsSucc { get; }
    public bool IsFail => !IsSucc;
    internal Error? Error => _error;
    internal T? Value => _value;
    
    public R Match<R>(Func<Error, R> fail, Func<T, R> succ)
        => IsSucc ? succ(_value!) : fail(_error!);

    public Unit Match(Action<Error> fail, Action<T> succ)
        => Match(fail.ToFunc(), succ.ToFunc());

    public override string ToString()
        => Match(
            fail => $"Error({fail.Message})", 
            succ => $"Succ({succ})");

    public Maybe<T> ToMaybe()
        => Match(_ => Nothing, Just);
    
    public Result<R> Map<R>(Func<T, R> f)
        => Match(
            fail => new Result<R>(fail),
            succ => f(succ));

    public Result<Unit> ForEach(Action<T> f)
        => Map(f.ToFunc());

    public Result<R> Bind<R>(Func<T, Result<R>> f)
        => Match(fail => new(fail), f);
    
    public Result<R> Select<R>(Func<T, R> f)
        => Map(f);

    public Result<PR> SelectMany<R, PR>(Func<T, Result<R>> bind, Func<T, R, PR> proj)
        => Match(
            ex => new(ex),
            t => bind(t).Match(
                ex => new Result<PR>(ex),
                r => proj(t, r)));
    
    internal Result<R> Cast<R>()
        => IsSucc
            ? F.Cast<R>(Value!)
                .Map(Result<R>.Succ)
                .IfNothing(() => Result<R>.Fail(new(
                    $"Can't cast success value of type {nameof(T)} to {nameof(R)}")))
            : Result<R>.Fail(_error!);
}

public static class ResultExt {
    public static Result<R> Apply<T, R>(this Result<Func<T, R>> self, Result<T> arg)
        => self.Match(
            fail => fail,
            f => arg.Match<Result<R>>(
                fail => fail,
                t => f(t)));

    public static Result<Func<T2, R>> Apply<T1, T2, R>(this Result<Func<T1, T2, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    public static Result<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Result<Func<T1, T2, T3, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    public static Result<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Result<Func<T1, T2, T3, T4, R>> self, 
        Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    public static Result<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>(
        this Result<Func<T1, T2, T3, T4, T5, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    public static Result<Func<T2, T3, T4, T5, T6, R>> Apply<T1, T2, T3, T4, T5, T6, R>(
        this Result<Func<T1, T2, T3, T4, T5, T6, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);

    public static Result<Func<T2, T3, T4, T5, T6, T7, R>> Apply<T1, T2, T3, T4, T5, T6, T7, R>(
        this Result<Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Result<T1> arg)
        => Apply(self.Map(F.CurryFirst), arg);
}