using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLibrary; 

public delegate (T Value, S State) Stateful<S, T>(S state);

public static class StatefulExt {
    public static T Run<S, T>(this Stateful<S, T> f, S state) 
        => f(state).Value;
    
    public static Stateful<S, R> Bind<S, T, R>(this Stateful<S, T> f, Func<T, Stateful<S, R>> proj)
        => state => {
            (T t, S newState) = f(state);
            
            return proj(t)(newState);
        };

    public static Stateful<S, R> Map<S, T, R>(this Stateful<S, T> f, Func<T, R> proj)
        => state => {
            (T t, S newState) = f(state);
            
            return (proj(t), newState);
        };

    // public static IEnumerable<T> AsEnumerable<T, S>(this Stateful<T, S> self)

    // public static Stateful<S, R> Bind<S, T, R>(this Stateful<S, T> self, Func<T, Stateful<S, R>> f)


}