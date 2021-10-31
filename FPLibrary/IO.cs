using System;

namespace FPLibrary; 

public static partial class F {
    public static IO<T> IO<T>(Func<T> f) => new(() => f());
}

public class IO<T> {
    private readonly Func<Exceptional<T>> _f;

    internal IO(Func<Exceptional<T>> f) => _f = f;

    public Exceptional<T> Run() {
        try {
            return _f();
        } catch (Exception ex) {
            return ex;
        }
    }

    public IO<R> Map<R>(Func<T, R> f)
        => new(() => Run()
            .Match<Exceptional<R>>(ex => ex, t => f(t)));

    public IO<R> Bind<R>(Func<T, IO<R>> f)
        => new(() => Run()
            .Match(ex => ex, t => f(t).Run()));

    public IO<R> Select<R>(Func<T, R> f)
        => Map(f);

    public IO<PR> SelectMany<R, PR>(Func<T, IO<R>> f, Func<T, R, PR> proj)
        => new(() => Run()
            .Match(ex => ex, t => f(t).Run()
                .Match<Exceptional<PR>>(ex => ex, r => proj(t, r))));
}