using System;

namespace FunctionalSharp; 

public delegate dynamic Middleware<T>(Func<T, dynamic> cont);

public static class Middleware {
    public static Middleware<R> Bind<T, R>(this Middleware<T> mw, Func<T, R> f) 
        => cont => mw(t => cont(f(t)));
}