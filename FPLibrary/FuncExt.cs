namespace FPLibrary;

using static F;

public static class FuncExt {
    public static Func<T> ToNullary<T>(this Func<Unit, T> f)
        => () => f(Unit());

    public static Func<T, bool> Negate<T>(this Func<T, bool> pred) => t => !pred(t);

    public static Func<T2, T1, R> Flip<T1, T2, R>(this Func<T1, T2, R> f) => (t2, t1) => f(t1, t2);

    public static Action<T2, T1> Flip<T1, T2>(this Action<T1, T2> f) => (t2, t1) => f(t1, t2);

    public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> f, T1 t1)
        => t2 => f(t1, t2);

    public static Func<T2, T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> f, T1 t1)
        => (t2, t3) => f(t1, t2, t3);

    public static Func<T2, T3, T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f, T1 t1)
        => (t2, t3, t4) => f(t1, t2, t3, t4);

    public static Func<T2, T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> f, T1 t1)
        => (t2, t3, t4, t5) => f(t1, t2, t3, t4, t5);

    public static Func<T2, T3, T4, T5, T6, R> Apply<T1, T2, T3, T4, T5, T6, R>(
        this Func<T1, T2, T3, T4, T5, T6, R> f, T1 t1)
        => (t2, t3, t4, t5, t6) => f(t1, t2, t3, t4, t5, t6);

    public static Func<T2, T3, T4, T5, T6, T7, R> Apply<T1, T2, T3, T4, T5, T6, T7, R>(
        this Func<T1, T2, T3, T4, T5, T6, T7, R> f, T1 t1)
        => (t2, t3, t4, t5, t6, t7) => f(t1, t2, t3, t4, t5, t6, t7);

    //functor
    public static Func<R> Map<T, R>(this Func<T> self, Func<T, R> f)
        => () => f(self());

    public static Func<I1, R> Map<I1, T, R>(this Func<I1, T> self, Func<T, R> f)
        => i1 => f(self(i1));

    public static Func<I1, I2, R> Map<I1, I2, T, R>(this Func<I1, I2, T> self, Func<T, R> f)
        => (i1, i2) => f(self(i1, i2));

    public static Func<I1, I2, I3, R> Map<I1, I2, I3, T, R>(this Func<I1, I2, I3, T> self, Func<T, R> f)
        => (i1, i2, i3) => f(self(i1, i2, i3));

    public static Func<I1, I2, I3, I4, R> Map<I1, I2, I3, I4, T, R>(this Func<I1, I2, I3, I4, T> self, Func<T, R> f)
        => (i1, i2, i3, i4) => f(self(i1, i2, i3, i4));

    public static Func<I1, I2, I3, I4, I5, R> Map<I1, I2, I3, I4, I5, T, R>(this Func<I1, I2, I3, I4, I5, T> self,
        Func<T, R> f)
        => (i1, i2, i3, i4, i5) => f(self(i1, i2, i3, i4, i5));

    public static Func<I1, I2, I3, I4, I5, I6, R> Map<I1, I2, I3, I4, I5, I6, T, R>(
        this Func<I1, I2, I3, I4, I5, I6, T> self, Func<T, R> f)
        => (i1, i2, i3, i4, i5, i6) => f(self(i1, i2, i3, i4, i5, i6));

    public static Func<I1, I2, I3, I4, I5, I6, I7, R> Map<I1, I2, I3, I4, I5, I6, I7, T, R>(
        this Func<I1, I2, I3, I4, I5, I6, I7, T> self, Func<T, R> f)
        => (i1, i2, i3, i4, i5, i6, i7) => f(self(i1, i2, i3, i4, i5, i6, i7));
}