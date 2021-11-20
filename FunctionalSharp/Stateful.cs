using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionalSharp; 

public delegate (T Value, S State) Stateful<S, T>(S state);

public static partial class F {
    
}

public static class Stateful<S> {
    public static Stateful<S, T> Return<T>(T value)
        => state => (value, state);
}

public static class StatefulExt {
    public static T Run<S, T>(this Stateful<S, T> f, S state) 
        => f(state).Value;
    
    public static Stateful<S, R> Bind<S, T, R>(this Stateful<S, T> self, Func<T, Stateful<S, R>> f)
        => state => {
            (T t, S newState) = self(state);
            
            return f(t)(newState);
        };

    public static Stateful<S, R> Map<S, T, R>(this Stateful<S, T> self, Func<T, R> f)
        => state => {
            (T t, S newState) = self(state);
            
            return (f(t), newState);
        };

    public static Stateful<S, R> Select<S, T, R>(this Stateful<S, T> self, Func<T, R> f)
        => Map(self, f);

    public static Stateful<S, PR> SelectMany<S, T, R, PR>(this Stateful<S, T> self, Func<T, Stateful<S, R>> bind,
        Func<T, R, PR> proj)
        => self.Bind(t => bind(t).Map(r => proj(t, r)));

    public static IEnumerable<T> AsEnumerable<S, T>(this Stateful<S, T> self, S state) {
        yield return self(state).Value;
    }
}