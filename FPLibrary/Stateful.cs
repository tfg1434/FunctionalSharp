using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLibrary; 

public delegate (T Value, S State) Stateful<S, T>(S state);

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

    // public static IEnumerable<T> AsEnumerable<T, S>(this Stateful<T, S> self)

    // public static Stateful<S, R> Bind<S, T, R>(this Stateful<S, T> self, Func<T, Stateful<S, R>> f)


}