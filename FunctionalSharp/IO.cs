namespace FunctionalSharp; 

public static partial class F {
    public static IO<E, T> IO<E, T>(Func<E, Result<T>> f) where E : struct
        => new(Thunk<E, T>.Of(f));

    public static IO<E, T> IOLazy<E, T>(Func<E, T> f) where E : struct
        => new(Thunk<E, T>.Of(env => new(f(env))));

    public static IO<E, T> IOSucc<E, T>(T value) where E : struct
        => new(Thunk<E, T>.OfSucc(value));

    public static IO<E, T> IOFail<E, T>(Error error) where E : struct
        => new(Thunk<E, T>.OfFail(error));
}

public readonly struct IO<E, T> where E : struct {
    private readonly Thunk<E, T> _thunk;

    internal IO(Thunk<E, T> thunk) => _thunk = thunk;

    public static implicit operator IO<E, T>(Eff<T> eff) => IO<E, T>(_ => eff.ReRun());

    public Result<T> Run(E env) => _thunk.Value(env);

    public Result<T> ReRun(E env) => _thunk.ReValue(env);

    public IO<E, T> Clone() => new(_thunk.Clone());

    public IO<E, R> Map<R>(Func<T, R> f)
        => new(_thunk.Map(f));

    public IO<E, R> BiMap<R>(Func<T, R> succ, Func<Error, Error> fail)
        => new(_thunk.BiMap(succ, fail));

    public IO<E, R> Match<R>(Func<T, R> succ, Func<Error, R> fail) {
        var @this = this;

        return IOLazy<E, R>(env => {
            Result<T> res = @this.Run(env);
            
            return res.Match(fail, succ);
        });
    }

    public IO<E, R> Bind<R>(Func<T, IO<E, R>> f) {
        var @this = this;

        return new(Thunk<E, R>.Of(env => {
            Result<T> res = @this.Run(env);

            return res.Match(Result<R>.Of, t => f(t).Run(env));
        }));
    }

    public IO<E, T> IfFail(Func<Error, T> f) {
        var @this = this;
    
        return IO<E, T>(env => {
            var res = @this.ReRun(env);
    
            return res.Match(e => Result<T>.Of(f(e)), t => t);
        });
    }

    public IO<E, T> IfFail(Func<T> f) {
        var @this = this;
    
        return IO<E, T>(env => {
            var res = @this.ReRun(env);
    
            return res.Match(_ => Result<T>.Of(f()), t => t);
        });
    }

    public IO<E, T> IfFail(T value) {
        var @this = this;
        
        return IO<E, T>(env => {
            var res = @this.ReRun(env);
    
            return res.Match(_ => Result<T>.Of(value), t => t);
        });
    }

    public IO<E, R> Select<R>(Func<T, R> f) => Map(f);
    
    public IO<E, PR> SelectMany<R, PR>(Func<T, IO<E, R>> bind, Func<T, R, PR> proj)
        => Bind(t => bind(t).Map(r => proj(t, r)));
}
