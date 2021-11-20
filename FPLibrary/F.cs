using System;

namespace FPLibrary {
    public static partial class F {
        public static Unit Unit() => default;

        public static R Using<TDisp, R>(TDisp disposable, Func<TDisp, R> f)
            where TDisp : IDisposable {
            using (disposable) return f(disposable);
        }

        public static Unit Using<TDisp>(TDisp disposable, Action<TDisp> act)
            where TDisp : IDisposable => Using(disposable, act.ToFunc());

        //this is in here instead of FuncExt so you can do F.CurryFirst as non-extension method.
        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> f)
            => t1 => t2 => f(t1, t2);

        public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> f)
            => t1 => t2 => t3 => f(t1, t2, t3);

        public static Func<T1, Func<T2, Func<T3, Func<T4, R>>>> Curry<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f)
            => t1 => t2 => t3 => t4 => f(t1, t2, t3, t4);

        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> Curry<T1, T2, T3, T4, T5, R>(
            this Func<T1, T2, T3, T4, T5, R> f)
            => t1 => t2 => t3 => t4 => t5 => f(t1, t2, t3, t4, t5);

        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, R>>>>>> Curry<T1, T2, T3, T4, T5, T6, R>(
            this Func<T1, T2, T3, T4, T5, T6, R> f)
            => t1 => t2 => t3 => t4 => t5 => t6 => f(t1, t2, t3, t4, t5, t6);

        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, R>>>>>>> Curry<T1, T2, T3, T4, T5,
            T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> f)
            => t1 => t2 => t3 => t4 => t5 => t6 => t7 => f(t1, t2, t3, t4, t5, t6, t7);

        public static Func<T1, Func<T2, R>> CurryFirst<T1, T2, R>(this Func<T1, T2, R> self)
            => t1 => t2 => self(t1, t2);

        public static Func<T1, Func<T2, T3, R>> CurryFirst<T1, T2, T3, R>(this Func<T1, T2, T3, R> self)
            => t1 => (t2, t3) => self(t1, t2, t3);

        public static Func<T1, Func<T2, T3, T4, R>> CurryFirst<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> self)
            => t1 => (t2, t3, t4) => self(t1, t2, t3, t4);

        public static Func<T1, Func<T2, T3, T4, T5, R>> CurryFirst<T1, T2, T3, T4, T5, R>(
            this Func<T1, T2, T3, T4, T5, R> self)
            => t1 => (t2, t3, t4, t5) => self(t1, t2, t3, t4, t5);

        public static Func<T1, Func<T2, T3, T4, T5, T6, R>> CurryFirst<T1, T2, T3, T4, T5, T6, R>(
            this Func<T1, T2, T3, T4, T5, T6, R> self)
            => t1 => (t2, t3, t4, t5, t6) => self(t1, t2, t3, t4, t5, t6);

        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, R>> CurryFirst<T1, T2, T3, T4, T5, T6, T7, R>(
            this Func<T1, T2, T3, T4, T5, T6, T7, R> self)
            => t1 => (t2, t3, t4, t5, t6, t7) => self(t1, t2, t3, t4, t5, t6, t7);

        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, R>> CurryFirst<T1, T2, T3, T4, T5, T6, T7, T8, R>(
            this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> self)
            => t1 => (t2, t3, t4, t5, t6, t7, t8) => self(t1, t2, t3, t4, t5, t6, t7, t8);

        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, R>> CurryFirst<T1, T2, T3, T4, T5, T6, T7, T8, T9,
            R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> self)
            => t1 => (t2, t3, t4, t5, t6, t7, t8, t9) => self(t1, t2, t3, t4, t5, t6, t7, t8, t9);
    }
}