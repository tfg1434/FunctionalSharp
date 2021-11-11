using System;
using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary; 

public static partial class F {
    public static IO<T> IO<T>(Func<Result<T>> f) 
        => new(Thunk<T>.OfSucc(f));

    public static IO<T> IO<T>(Func<T> f)
        => new(Thunk<T>.OfSucc(() => new Result<T>(f())));

    public static IO<T> IO<T>(T value)
        => new(Thunk<T>.OfSucc(value));

    public static IO<T> IO<T>(Error error)
        => new(Thunk<T>.OfFail(error));
}

public readonly struct IO<T> {
    private readonly Thunk<T> _thunk;

    internal IO(Thunk<T> thunk) 
        => _thunk = thunk;

    public Result<T> Run() => _thunk.Value();
    
    public Result<T> ReRun() => _thunk.ReValue();

    public IO<T> Clone() => new(_thunk.Clone());

    public IO<R> Map<R>(Func<T, R> f)
        => new(_thunk.Map(f));

    public IO<R> BiMap<R>(Func<T, R> succ, Func<Error, Error> fail)
        => new(_thunk.BiMap(succ, fail));



    // private readonly Func<Exceptional<T>> _f;
    //
    // internal IO(Func<Exceptional<T>> f) => _f = f;
    //
    // public Exceptional<T> Run() {
    //     try {
    //         return _f();
    //     } catch (Exception ex) {
    //         return ex;
    //     }
    // }
    //
    // public IO<R> Map<R>(Func<T, R> f)
    //     => new(() => Run()
    //         .Match<Exceptional<R>>(ex => ex, t => f(t)));
    //
    // public IO<R> Bind<R>(Func<T, IO<R>> f)
    //     => new(() => Run()
    //         .Match(ex => ex, t => f(t).Run()));
    //
    // public IO<R> Select<R>(Func<T, R> f)
    //     => Map(f);
    //
    // public IO<PR> SelectMany<R, PR>(Func<T, IO<R>> f, Func<T, R, PR> proj)
    //     => new(() => Run()
    //         .Match(ex => ex, t => f(t).Run()
    //             .Match<Exceptional<PR>>(ex => ex, r => proj(t, r))));
}

public static class IOExt
{
    public static IO<R> Match<T, R>(this IO<T> self, Func<T, R> succ, Func<Error, R> fail)
        => IO(() => {
            Result<T> res = self.ReRun();

            return res.IsSucc
                ? succ(res.Value!)
                : fail(res.Error!);
        });
}