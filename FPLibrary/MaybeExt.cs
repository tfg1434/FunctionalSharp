using System;
using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary {
    public readonly partial struct Maybe<T> {
        public T NotNothing() => IsJust ? _value! : throw new InvalidOperationException();
        
        public T IfNothing(T val) {
            if (val is null) throw new ArgumentNullException(nameof(val));

            return IsJust ? _value! : val;
        }

        public T IfNothing(Func<T> f) {
            if (f is null) throw new ArgumentNullException(nameof(f));

            return IsJust ? _value! : f() ?? throw new ArgumentException("Callback returned null", nameof(f));
        }

        public Unit IfNothing(Action f) {
            if (IsNothing) f();

            return Unit();
        }
        
    }
}