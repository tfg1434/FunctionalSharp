using System;
using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary {
    public static class Fluency {
        public static R Pipe<T, R>(this T self, Func<T, R> f)
            => f(self);

        public static R Pipe<T1, T2, R>(this T1 self, Func<T1, T2> f1, Func<T2, R> f2)
            => f2(f1(self));

        public static Unit Pipe<T>(this T self, Action<T> f) {
            f(self);
            return Unit();
        }
    }
}