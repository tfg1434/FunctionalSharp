using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public static class NullableExt {
    /// <summary>
    /// Turn a nullable into a <see cref="Maybe{T}"/>
    /// </summary>
    [Pure]
    public static Maybe<T> ToMaybe<T>(this T? self) where T : struct
        => self.HasValue ? Just(self.Value) : Nothing;
}