﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
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

        public static Maybe<T> Head<T>(this IEnumerable<T> enumerable) {
            if (enumerable is null) return Nothing;
            var enumerator = enumerable.GetEnumerator();
            return enumerator.MoveNext() ? Just(enumerator.Current) : Nothing;
        }

        public static Maybe<IEnumerable<T>> Tail<T>(this IEnumerable<T> enumerable) {
            if (enumerable is null || !enumerable.Any()) return Nothing;
            return Just(enumerable.Skip(1));
        }
    }
}
