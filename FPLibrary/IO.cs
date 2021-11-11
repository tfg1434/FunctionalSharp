using System;
using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary; 

public static partial class F {
    public static IO<T> IO<T>(Func<Result<T>> f) 
        => new(Thunk<T>.Of(f));

    public static IO<T> IO<T>(Func<T> f)
        => new(Thunk<T>.Of(() => new(f())));

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

    public IO<R> Match<R>(Func<T, R> succ, Func<Error, R> fail) {
        var @this = this;
        
        return IO(() => {
            Result<T> res = @this.ReRun();

            return res.IsSucc
                ? succ(res.Value!)
                : fail(res.Error!);
        });
    }

    public IO<R> Bind<R>(Func<T, IO<R>> f) {
        var @this = this;
        
        return new(Thunk<R>.Of(() => {
            Result<T> res = @this.ReRun();

            if (res.IsFail)
                return Result<R>.Of(res.Error!);

            return f(res.Value!).Run();
        }));
    }

    public IO<T> IfFail(Func<Error, T> f) {
        var @this = this;
        
        return IO(() => {
            var res = @this.ReRun();
            
            return res.IsSucc ? res : Result<T>.Of(f(res.Error!));
        });
    }

    public IO<T> IfFail(Func<T> f) {
        var @this = this;
        
        return IO(() => {
            var res = @this.ReRun();
            
            return res.IsSucc ? res : Result<T>.Of(f());
        });
    }
    
    public IO<T> IfFail(T value) {
        var @this = this;
        
        return IO(() => {
            var res = @this.ReRun();
            
            return res.IsSucc ? res : Result<T>.Of(value);
        });
    }

    public IO<R> Select<R>(Func<T, R> f) => Map(f);
    
    public IO<PR> SelectMany<R, PR>(Func<T, IO<R>> bind, Func<T, R, PR> proj)
        => Bind(t => bind(t).Map(r => proj(t, r)));
}

public static class IOExt
{
    
}