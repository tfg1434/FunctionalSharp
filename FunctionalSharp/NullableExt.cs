using System;
using FunctionalSharp;
using static FunctionalSharp.F;

namespace FunctionalSharp; 

public static class NullableExt {
    public static Maybe<T> ToMaybe<T>(this T? self) where T : struct
        => self.HasValue ? Just(self.Value) : Nothing;
}