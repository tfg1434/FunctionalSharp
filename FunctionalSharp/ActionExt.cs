using System.Diagnostics.Contracts;

namespace FunctionalSharp;

using static F;

/// <summary>
/// <see cref="Action"/> extensions
/// </summary>
public static class ActionExt {
    /// <summary>
    /// Convert an Action to a Unit-returning Func
    /// </summary>
    /// <param name="f">Action to convert</param>
    [Pure]
    public static Func<Unit> ToFunc(this Action f)
        => () => {
            f();

            return Unit();
        };

    /// <summary>
    /// Convert an Action to a Unit-returning Func
    /// </summary>
    /// <param name="f">Action to convert</param>
    [Pure]
    public static Func<T, Unit> ToFunc<T>(this Action<T> f)
        => t => {
            f(t);

            return Unit();
        };

    /// <summary>
    /// Convert an Action to a Unit-returning Func
    /// </summary>
    /// <param name="f">Action to convert</param>
    [Pure]
    public static Func<T1, T2, Unit> ToFunc<T1, T2>(this Action<T1, T2> f)
        => (t1, t2) => {
            f(t1, t2);

            return Unit();
        };

    /// <summary>
    /// Convert an Action to a Unit-returning Func
    /// </summary>
    /// <param name="f">Action to convert</param>
    [Pure]
    public static Func<T1, T2, T3, Unit> ToFunc<T1, T2, T3>(this Action<T1, T2, T3> f)
        => (t1, t2, t3) => {
            f(t1, t2, t3);

            return Unit();
        };

    /// <summary>
    /// Convert an Action to a Unit-returning Func
    /// </summary>
    /// <param name="f">Action to convert</param>
    [Pure]
    public static Func<T1, T2, T3, T4, Unit> ToFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> f)
        => (t1, t2, t3, t4) => {
            f(t1, t2, t3, t4);

            return Unit();
        };

    /// <summary>
    /// Convert an Action to a Unit-returning Func
    /// </summary>
    /// <param name="f">Action to convert</param>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, Unit> ToFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> f)
        => (t1, t2, t3, t4, t5) => {
            f(t1, t2, t3, t4, t5);

            return Unit();
        };

    /// <summary>
    /// Convert an Action to a Unit-returning Func
    /// </summary>
    /// <param name="f">Action to convert</param>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, Unit> ToFunc<T1, T2, T3, T4, T5, T6>(
        this Action<T1, T2, T3, T4, T5, T6> f)
        => (t1, t2, t3, t4, t5, t6) => {
            f(t1, t2, t3, t4, t5, t6);

            return Unit();
        };

    /// <summary>
    /// Convert an Action to a Unit-returning Func
    /// </summary>
    /// <param name="f">Action to convert</param>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> ToFunc<T1, T2, T3, T4, T5, T6, T7>(
        this Action<T1, T2, T3, T4, T5, T6, T7> f)
        => (t1, t2, t3, t4, t5, t6, t7) => {
            f(t1, t2, t3, t4, t5, t6, t7);

            return Unit();
        };
}