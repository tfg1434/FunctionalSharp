using System;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary; 

public static class NullableExt {
    public static Maybe<T> ToMaybe<T>(this T? self) where T : struct
        => self.HasValue ? Just(self.Value) : Nothing;
}