using System;
using System.Collections.Generic;
using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary {
    public static partial class F {
        public static Exceptional<T> Exceptional<T>(T t) => new(t);
    }

    public readonly struct Exceptional<T> {
        private Exception? ex { get; }
        private T? value { get; }

        private bool isSuccess { get; }
        private bool isEx => !isSuccess;

        internal Exceptional(Exception ex) {
            isSuccess = false;
            this.ex = ex;
            value = default;
        }

        internal Exceptional(T value) {
            isSuccess = true;
            this.value = value;
            ex = default;
        }

        public static implicit operator Exceptional<T>(Exception ex) => new(ex);
        public static implicit operator Exceptional<T>(T t) => new(t);

        public R Match<R>(Func<Exception, R> fEx, Func<T, R> fSuccess)
            => isEx ? fEx(ex!) : fSuccess(value!);

        public Unit Match(Action<Exception> fEx, Action<T> fSuccess)
            => Match(fEx.ToFunc(), fSuccess.ToFunc());

        public override string ToString()
            => Match(
                exception => $"Exception({exception.Message})",
                t => $"Success({t})");
    }
}
