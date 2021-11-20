using System;

namespace FunctionalSharp {
    public static partial class F {
        public static Func<R> Fun<R>(Func<R> f) => f;
        
        public static Func<T1, R> Fun<T1, R>(Func<T1, R> f) => f;
        
        public static Func<T1, T2, R> Fun<T1, T2, R>(Func<T1, T2, R> f) => f;
        
        public static Func<T1, T2, T3, R> Fun<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => f;

        public static Func<T1, T2, T3, T4, R> Fun<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) => f;
        
        public static Func<T1, T2, T3, T4, T5, R> Fun<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) => f;
        
        public static Func<T1, T2, T3, T4, T5, T6, R> Fun<T1, T2, T3, T4, T5, T6, R>
            (Func<T1, T2, T3, T4, T5, T6, R> f) => f;
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, R> Fun<T1, T2, T3, T4, T5, T6, T7, R>
            (Func<T1, T2, T3, T4, T5, T6, T7, R> f) => f;
        
        public static Action Act(Action f) => f;
        
        public static Action<T1> Act<T1>(Action<T1> f) => f;
        
        public static Action<T1, T2> Act<T1, T2>(Action<T1, T2> f) => f;
        
        public static Action<T1, T2, T3> Act<T1, T2, T3>(Action<T1, T2, T3> f) => f;

        public static Action<T1, T2, T3, T4> Act<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f) => f;
        
        public static Action<T1, T2, T3, T4, T5> Act<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> f) => f;
        
        public static Action<T1, T2, T3, T4, T5, T6> Act<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> f) => f;
        
        public static Action<T1, T2, T3, T4, T5, T6, T7> Act<T1, T2, T3, T4, T5, T6, T7>
            (Action<T1, T2, T3, T4, T5, T6, T7> f) => f;
    }
}