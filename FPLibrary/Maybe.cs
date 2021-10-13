using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unit = System.ValueTuple;

namespace FPLibrary {
    using static F;

    public static partial class F {
        public static NothingType Nothing => default;

        public static Maybe<T> Just<T>([NotNull] T? value)
            => new(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public readonly struct NothingType { }

    public readonly struct Maybe<T> : IEquatable<NothingType>, IEquatable<Maybe<T>> {
        private bool isNothing => !IsJust;
        private readonly T? value;

        public bool IsJust { get; }

        public bool IsNothing => isNothing;


        internal Maybe(T t) {
            IsJust = true;
            value = t;
        }

        public static implicit operator Maybe<T>(NothingType _) => default;

        public static implicit operator Maybe<T>(T? t)
            => t is null ? Nothing : Just(t);

        public R Match<R>(Func<R> nothing, Func<T, R> just)
            => IsJust ? just(value!) : nothing();

        public IEnumerable<T> AsEnumerable() {
            if (IsJust) yield return value!;
        }

        public T IfNothing(T val) {
            if (val is null) throw new ArgumentNullException(nameof(val));

            return IsJust ? value! : val;
        }

        public T IfNothing(Func<T> f)
            => (IsJust
                ? value
                : f()) ?? throw new InvalidOperationException();

        public Unit IfNothing(Action f) {
            if (IsNothing) f();

            return Unit();
        }

        public bool Equals(Maybe<T> other)
            => IsJust == other.IsJust && (!IsJust || value!.Equals(other.value));

        public bool Equals(NothingType _) => !IsJust;

        public override bool Equals(object? other)
            => other is Maybe<T> && Equals(other);

        public override int GetHashCode()
            => IsJust ? value!.GetHashCode() : 0;

        public static bool operator ==(Maybe<T> self, Maybe<T> other) => self.Equals(other);

        public static bool operator !=(Maybe<T> self, Maybe<T> other) => !(self == other);

        public override string ToString() => IsJust ? $"Just({value})" : "Nothing";
    }

    //namespace Maybe {
    //    //you can instantiate but not access internal fields.
    //    public readonly struct Nothing { }

    //    public readonly struct Just<T> {
    //        internal T Value { get; }

    //        internal Just(T value) {
    //            if (value is null)
    //                throw new ArgumentNullException(nameof(value), 
    //                    "Cannot wrap null in Just. Did you mean to use Nothing");
    //            Value = value;
    //        }
    //    }
    //}

    public static class MaybeExt {
        public static Maybe<R> Map<T, R>(this NothingType _, Func<T, R> __) => Nothing;

        public static Maybe<R> Map<T, R>(this Maybe<T> maybeT, Func<T, R> f)
            => maybeT.Match(() => Nothing, t => Just(f(t)));

        public static Maybe<Func<T2, R>> Map<T1, T2, R>(this Maybe<T1> self, Func<T1, T2, R> f)
            => self.Map(f.CurryFirst());

        public static Maybe<Func<T2, T3, R>> Map<T1, T2, T3, R>(this Maybe<T1> self, Func<T1, T2, T3, R> f)
            => self.Map(f.CurryFirst());

        public static Maybe<Func<T2, T3, T4, R>> Map<T1, T2, T3, T4, R>(this Maybe<T1> self, Func<T1, T2, T3, T4, R> f)
            => self.Map(f.CurryFirst());

        public static Maybe<Func<T2, T3, T4, T5, R>> Map<T1, T2, T3, T4, T5, R>(this Maybe<T1> self,
            Func<T1, T2, T3, T4, T5, R> f)
            => self.Map(f.CurryFirst());

        public static Maybe<Func<T2, T3, T4, T5, T6, R>> Map<T1, T2, T3, T4, T5, T6, R>(this Maybe<T1> self,
            Func<T1, T2, T3, T4, T5, T6, R> f)
            => self.Map(f.CurryFirst());

        public static Maybe<Func<T2, T3, T4, T5, T6, T7, R>> Map<T1, T2, T3, T4, T5, T6, T7, R>(this Maybe<T1> self,
            Func<T1, T2, T3, T4, T5, T6, T7, R> f)
            => self.Map(f.CurryFirst());

        public static Maybe<Unit> ForEach<T>(this Maybe<T> maybe, Action<T> act)
            => Map(maybe, act.ToFunc());

        public static Maybe<R> Bind<T, R>(this Maybe<T> maybeT, Func<T, Maybe<R>> f)
            => maybeT.Match(() => Nothing, f);

        //give IEnumerable<R> instead of Maybe<IEnumerable<R>>
        public static IEnumerable<R> Bind<T, R>(this Maybe<T> maybe, Func<T, IEnumerable<R>> f)
            => maybe.AsEnumerable().Bind(f);

        public static Either<L, R> ToEither<L, R>(this Maybe<R> self, Func<L> left)
            => self.Match<Either<L, R>>(() => left(), r => r);

        public static Either<L, R> ToEither<L, R>(this Maybe<R> self, L defaultLeftVal)
            => self.Match<Either<L, R>>(() => defaultLeftVal, r => r);

        //function application
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

        //utilities
        public static Unit Match<T>(this Maybe<T> self, Action nothing, Action<T> just)
            => self.Match(nothing.ToFunc(), just.ToFunc());

        public static T GetOrElse<T>(this Maybe<T> maybeT, T defaultVal)
            => maybeT.Match(() => defaultVal, t => t);

        public static T GetOrElse<T>(this Maybe<T> maybeT, Func<T> defaultVal)
            => maybeT.Match(defaultVal, t => t);

        //query syntax
        public static Maybe<R> Select<T, R>(this Maybe<T> self, Func<T, R> f) => self.Map(f);

        public static Maybe<RR> SelectMany<T, R, RR>(this Maybe<T> maybe, Func<T, Maybe<R>> bind, Func<T, R, RR> proj)
            => maybe.Match(
                () => Nothing,
                t => bind(t).Match(
                    () => Nothing,
                    r => Just(proj(t, r))));

        public static Maybe<T> Where<T>(this Maybe<T> maybeT, Predicate<T> pred)
            => maybeT.Match(
                () => Nothing,
                t => pred(t) ? maybeT : Nothing);
    }
}