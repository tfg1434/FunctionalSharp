using System;
using System.Collections.Generic;
using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary {
    public static class MaybeExt {
        public static Maybe<R> Map<T, R>(this NothingType _, Func<T, R> __) => Nothing;
    
        //extension methods for type constraints
        //apply a Maybe<T> to a Maybe<Func<T, R>> if both are Just
        public static Maybe<R> Apply<T, R>(this Maybe<Func<T, R>> self, Maybe<T> arg)
            => self.Match(
                () => Nothing,
                f => arg.Match(
                    () => Nothing,
                    val => Just(f(val))));

        public static Maybe<Func<T2, R>> Apply<T1, T2, R>(this Maybe<Func<T1, T2, R>> self, Maybe<T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Maybe<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Maybe<Func<T1, T2, T3, R>> self, Maybe<T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Maybe<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Maybe<Func<T1, T2, T3, T4, R>> self,
            Maybe<T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Maybe<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>(
            this Maybe<Func<T1, T2, T3, T4, T5, R>> self, Maybe<T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Maybe<Func<T2, T3, T4, T5, T6, R>> Apply<T1, T2, T3, T4, T5, T6, R>(
            this Maybe<Func<T1, T2, T3, T4, T5, T6, R>> self, Maybe<T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Maybe<Func<T2, T3, T4, T5, T6, T7, R>> Apply<T1, T2, T3, T4, T5, T6, T7, R>(
            this Maybe<Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Maybe<T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);
    }

    public readonly partial struct Maybe<T> {
        public T NotNothing() 
            => Match(() => throw new InvalidOperationException(), t => t);
        
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
        
        public Maybe<R> Map<R>(Func<T, R> f)
            => Match(() => Nothing, t => Just(f(t)));

        public Maybe<Func<T2, R>> Map<T2, R>(Func<T, T2, R> f)
            => Map(f.CurryFirst());

        public Maybe<Func<T2, T3, R>> Map<T2, T3, R>(Func<T, T2, T3, R> f)
            => Map(f.CurryFirst());

        public Maybe<Func<T2, T3, T4, R>> Map<T2, T3, T4, R>(Func<T, T2, T3, T4, R> f)
            => Map(f.CurryFirst());

        public Maybe<Func<T2, T3, T4, T5, R>> Map<T2, T3, T4, T5, R>(
            Func<T, T2, T3, T4, T5, R> f)
            => Map(f.CurryFirst());

        public Maybe<Func<T2, T3, T4, T5, T6, R>> Map<T2, T3, T4, T5, T6, R>(
            Func<T, T2, T3, T4, T5, T6, R> f)
            => Map(f.CurryFirst());

        public Maybe<Func<T2, T3, T4, T5, T6, T7, R>> Map<T2, T3, T4, T5, T6, T7, R>(
            Func<T, T2, T3, T4, T5, T6, T7, R> f)
            => Map(f.CurryFirst());
        
        public Maybe<Unit> ForEach(Action<T> act)
            => Map(act.ToFunc());

        public Maybe<R> Bind<R>(Func<T, Maybe<R>> f)
            => Match(() => Nothing, f);
        
        //give IEnumerable<R> instead of Maybe<IEnumerable<R>>
        public IEnumerable<R> Bind<R>(Func<T, IEnumerable<R>> f)
            => AsEnumerable().Bind(f);
        
        public Either<L, T> ToEither<L>(Func<L> f)
            => Match<Either<L, T>>(() => f(), r => r);

        public Either<L, T> ToEither<L>(L defaultLeft)
            => Match<Either<L, T>>(() => defaultLeft, r => r);//Match<Either<L, R>>(() => defaultLeft, r => r);

        //utilities
        public Unit Match(Action nothing, Action<T> just)
            => Match(nothing.ToFunc(), just.ToFunc());

        public T GetOr(T defaultVal)
            => Match(() => defaultVal, t => t);

        public T GetOr(Func<T> defaultVal)
            => Match(defaultVal, t => t);

        //query syntax
        public Maybe<R> Select<R>(Func<T, R> f) => Map(f);

        public Maybe<RR> SelectMany<R, RR>(Func<T, Maybe<R>> bind, Func<T, R, RR> proj)
            => Match(
                () => Nothing,
                t => bind(t).Match(
                    () => Nothing,
                    r => Just(proj(t, r))));

        public Maybe<T> Where(Func<T, bool> p) {
            var self = this;
            
            return Match(
                () => Nothing,
                t => p(t) ? self : Nothing);
        }
    }
}