using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

/// <summary>
/// C# fluency extensions
/// </summary>
public static class Fluency {
    /// <summary>
    /// Similar to F# forward pipe operator
    /// </summary>
    [Pure]
    public static R Pipe<T, R>(this T self, Func<T, R> f)
        => f(self);

    /// <summary>
    /// Pipe two functions
    /// </summary>
    [Pure]
    public static R Pipe<T1, T2, R>(this T1 self, Func<T1, T2> f1, Func<T2, R> f2)
        => f2(f1(self));

    /// <summary>
    /// Pipe an action
    /// </summary>
    [Pure]
    public static Unit Pipe<T>(this T self, Action<T> f) {
        f(self);

        return Unit();
    }
}