using System;
using Unit = System.ValueTuple;

namespace FPLibrary {
    public static partial class F {
        public static Unit Unit() => default;

        public static R Using<TDisp, R>(TDisp disposable, Func<TDisp, R> f) 
            where TDisp : IDisposable {

            using (disposable) 
                return f(disposable);
        }

        public static Unit Using<TDisp>(TDisp disposable, Action<TDisp> act)
            where TDisp : IDisposable => Using(disposable, act.ToFunc());
    }
}
