using System;
using Unit = System.ValueTuple;

namespace FPLibrary {
    using static F;

    public static class FuncExt {
        public static Func<T1, R> Compose<T1, T2, R>(this Func<T2, R> g, Func<T1, T2> f)
            => x => g(f(x));

        public static Predicate<T> Negate<T>(this Predicate<T> pred) => t => !pred(t);

        public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> f, T1 t1)
            => t2 => f(t1, t2);

        public static Func<T2, T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> f, T1 t1)
            => (t2, t3) => f(t1, t2, t3);

        public static Func<T2, T3, T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f, T1 t1)
            => (t2, t3, t4) => f(t1, t2, t3, t4);

        public static Func<T2, T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> f, T1 t1)
            => (t2, t3, t4, t5) => f(t1, t2, t3, t4, t5);

        public static Func<T2, T3, T4, T5, T6, R> Apply<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> f, T1 t1)
            => (t2, t3, t4, t5, t6) => f(t1, t2, t3, t4, t5, t6);

        public static Func<T2, T3, T4, T5, T6, T7, R> Apply<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> f, T1 t1)
            => (t2, t3, t4, t5, t6, t7) => f(t1, t2, t3, t4, t5, t6, t7);

        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> f)
            => t1 => t2 => f(t1, t2);

        public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> f)
            => t1 => t2 => t3 => f(t1, t2, t3);

        public static Func<T1, Func<T2, Func<T3, Func<T4, R>>>> Curry<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f)
            => t1 => t2 => t3 => t4 => f(t1, t2, t3, t4);

        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> Curry<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> f)
            => t1 => t2 => t3 => t4 => t5 => f(t1, t2, t3, t4, t5);

        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, R>>>>>> Curry<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> f)
            => t1 => t2 => t3 => t4 => t5 => t6 => f(t1, t2, t3, t4, t5, t6);

        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, R>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> f)
            => t1 => t2 => t3 => t4 => t5 => t6 => t7 => f(t1, t2, t3, t4, t5, t6, t7);
    }
}
