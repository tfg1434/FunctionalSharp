using System;
using System.Collections.Generic;
using OneOf.Types;
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

    public static class Exceptional {
        //functor
        public static Exceptional<R> Map<T, R>(this Exceptional<T> self, Func<T, R> f)
            => self.Match(
                ex => new Exceptional<R>(ex),
                r => f(r));

        public static Exceptional<Unit> ForEach<T>(this Exceptional<T> self, Action<T> act)
            => Map(self, act.ToFunc());

        public static Exceptional<R> Bind<T, R>(this Exceptional<T> self, Func<T, Exceptional<R>> f)
            => self.Match(
                ex => new Exceptional<R>(ex), f);

        //monadic return
        public static Func<T, Exceptional<T>> Return<T>()
            => t => t;

        public static Exceptional<R> Of<R>(Exception left)
            => new(left);

        public static Exceptional<R> Of<R>(R right)
            => new(right);

        //applicative
        public static Exceptional<R> Apply<T, R>(this Exceptional<Func<T, R>> self, Exceptional<T> arg)
            => self.Match(
                ex => ex,
                func => arg.Match(
                   ex => ex,
                   t => F.Exceptional(func(t))));

        public static Exceptional<Func<T2, R>> Apply<T1, T2, R>(this Exceptional<Func<T1, T2, R>> self, Exceptional<T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Exceptional<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Exceptional<Func<T1, T2, T3, R>> self, Exceptional<T1> arg)
           => Apply(self.Map(F.CurryFirst), arg);

        public static Exceptional<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Exceptional<Func<T1, T2, T3, T4, R>> self, Exceptional<T1> arg)
           => Apply(self.Map(F.CurryFirst), arg);

        public static Exceptional<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>(this Exceptional<Func<T1, T2, T3, T4, T5, R>> self, Exceptional<T1> arg)
           => Apply(self.Map(F.CurryFirst), arg);

        public static Exceptional<Func<T2, T3, T4, T5, T6, R>> Apply<T1, T2, T3, T4, T5, T6, R>(this Exceptional<Func<T1, T2, T3, T4, T5, T6, R>> self, Exceptional<T1> arg)
           => Apply(self.Map(F.CurryFirst), arg);

        public static Exceptional<Func<T2, T3, T4, T5, T6, T7, R>> Apply<T1, T2, T3, T4, T5, T6, T7, R>(this Exceptional<Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Exceptional<T1> arg)
           => Apply(self.Map(F.CurryFirst), arg);

        //QOL
        public static Maybe<T> ToMaybe<T>(this Exceptional<T> self)
            => self.Match(_ => Nothing, Just);

        //query syntax
        public static Exceptional<R> Select<T, R>(this Exceptional<T> self, Func<T, R> f)
            => self.Map(f);

        public static Exceptional<RR> SelectMany<T, R, RR>(this Exceptional<T> self, Func<T, Exceptional<R>> bind, Func<T, R, RR> project)
            => self.Match(
                ex => new Exceptional<RR>(ex),
                t => bind(t).Match(
                    ex => new Exceptional<RR>(ex),
                    r => project(t, r)));
    }
}
