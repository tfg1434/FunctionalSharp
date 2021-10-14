using System;
using System.Collections.Generic;
using System.Linq;
using Unit = System.ValueTuple;

namespace FPLibrary {
    using static F;

    public static class EnumberableExt {
        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> enumerable, Func<T, R> f)
            => enumerable.Select(f);

        public static void ForEach<T>(this IEnumerable<T> ts, Action<T> act) {
            foreach (T t in ts) act(t);
        }

        public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> ts, Func<T, IEnumerable<R>> f)
            => ts.SelectMany(f);

        //give IEnumerable<R> instead of IEnumerable<Maybe<R>> (Map)
        public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> ts, Func<T, Maybe<R>> f)
            => ts.Bind(t => f(t).AsEnumerable());

        public static Maybe<T> First<T>(this IEnumerable<T> ts, Func<T, bool> p) {
            foreach (T t in ts)
                if (p(t))
                    return t;

            return Nothing;
        }
    }
}