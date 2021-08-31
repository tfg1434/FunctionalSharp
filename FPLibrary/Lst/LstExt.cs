using System;
using System.Globalization;
using System.Linq;

namespace FPLibrary {
    using static F;

    public static class LstExt {
        public static R Match<T, R>(this Lst<T> list, Func<R> empty, Func<T, Lst<T>, R> cons) {
            if (list.Count == 0) return empty();

            (T t, Lst<T> ts) = list;
            return cons(t, ts);
        }

        public static Lst<R> Map<T, R>(this Lst<T> list, Func<T, R> f)
            => list.Match(
                () => List<R>(),
                (t, ts) => List(f(t), ts.Map(f)));
    }
}
