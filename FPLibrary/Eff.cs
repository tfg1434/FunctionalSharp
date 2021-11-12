using System;
using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary; 

public static partial class F {
    public static Eff<T> Eff<T>(Func<Result<T>> f) 
        => new(Thunk<T>.Of(f));

    public static Eff<T> EffLazy<T>(Func<T> f)
        => new(Thunk<T>.Of(() => new(f())));

    public static Eff<T> EffSucc<T>(T value)
        => new(Thunk<T>.OfSucc(value));

    public static Eff<T> EffFail<T>(Error error)
        => new(Thunk<T>.OfFail(error));
}

public readonly struct Eff<T> {
    private readonly Thunk<T> _thunk;

    internal Eff(Thunk<T> thunk) 
        => _thunk = thunk;

    public Result<T> Run() => _thunk.Value();
    
    public Result<T> ReRun() => _thunk.ReValue();

    public Eff<T> Clone() => new(_thunk.Clone());

    public Eff<R> Map<R>(Func<T, R> f)
        => new(_thunk.Map(f));

    public Eff<R> BiMap<R>(Func<T, R> succ, Func<Error, Error> fail)
        => new(_thunk.BiMap(succ, fail));

    public Eff<R> Match<R>(Func<T, R> succ, Func<Error, R> fail) {
        var @this = this;
        
        return EffLazy(() => {
            Result<T> res = @this.ReRun();

            return res.Match(fail, succ);
        });
    }

    public Eff<R> Bind<R>(Func<T, Eff<R>> f) {
        var @this = this;
        
        return new(Thunk<R>.Of(() => {
            Result<T> res = @this.ReRun();

            return res.Match(Result<R>.Of, t => f(t).Run());
        }));
    }

    public Eff<T> IfFail(Func<Error, T> f) {
        var @this = this;
        
        return Eff(() => {
            var res = @this.ReRun();

            return res.Match(e => Result<T>.Of(f(e)), t => t);
        });
    }

    public Eff<T> IfFail(Func<T> f) {
        var @this = this;
        
        return Eff(() => {
            var res = @this.ReRun();
            
            return res.Match(_ => Result<T>.Of(f()), t => t);
        });
    }
    
    public Eff<T> IfFail(T value) {
        var @this = this;
        
        return Eff(() => {
            var res = @this.ReRun();
            
            return res.Match(_ => Result<T>.Of(value), t => t);
        });
    }

    public Eff<R> Select<R>(Func<T, R> f) => Map(f);
    
    public Eff<PR> SelectMany<R, PR>(Func<T, Eff<R>> bind, Func<T, R, PR> proj)
        => Bind(t => bind(t).Map(r => proj(t, r)));
}
