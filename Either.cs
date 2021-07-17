using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unit = System.ValueTuple;

namespace FPLibrary {
    using static F;

    public static partial class F {
        public static Either.Left<L> Left<L>(L l) => new(l);
        public static Either.Right<R> Right<R>(R r) => new(r);
    }
    
    public readonly struct Either<L, R> {
        internal readonly L Left;
        internal readonly R Right;

        private readonly bool IsRight;
        private bool IsLeft => !IsRight;

        internal Either(L left)
            => (IsRight, Left, Right)
                = (false, left ?? throw new ArgumentNullException(nameof(left)), default);

        internal Either(R right)
            => (IsRight, Left, Right)
                = (true, default, right ?? throw new ArgumentNullException(nameof(right)));

        public static implicit operator Either<L, R>(L left) => new(left);
        public static implicit operator Either<L, R>(R right) => new(right);
        public static implicit operator Either<L, R>(Either.Left<L> left) => new(left.Value);
        public static implicit operator Either<L, R>(Either.Right<R> right) => new(right.Value);

        public TR Match<TR>(Func<L, TR> left, Func<R, TR> right)
            => IsRight ? right(Right) : left(Left);

        public Unit Match(Action<L> left, Action<R> right) 
            => Match(left.ToFunc(), right.ToFunc());

        public IEnumerable<R> AsEnumerable() {
            if (IsRight) yield return Right;
        }

        public override string ToString() => Match(l => $"Left({l})", r => $"Right({r})");
    }

    public static class Either {
        public readonly struct Left<L> {
            internal L Value { get; }
            internal Left(L value) => Value = value;

            public override string ToString() => $"Left({Value})";
        }

        public readonly struct Right<R> {
            internal R Value { get; }
            internal Right(R value) => Value = value;

            public override string ToString() => $"Right({Value})";
        }
    }

    public static class EitherExt {
        public static Either<L, RR> Map<L, R, RR>(this Either<L, R> self, Func<R, RR> f)
            // ReSharper disable once ConvertClosureToMethodGroup
            => self.Match<Either<L, RR>>(l => Left(l), r => Right(f(r)));

        //modify both Left and Right
        //called biftor or biMap
        public static Either<LL, RR> Map<L, LL, R, RR>(this Either<L, R> self, Func<L, LL> left, Func<R, RR> right)
            => self.Match<Either<LL, RR>>(l => Left(left(l)), r => Right(right(r)));

        public static Either<L, Unit> ForEach<L, R>(this Either<L, R> either, Action<R> act)
            => either.Map(act.ToFunc());

        public static Either<L, RR> Bind<L, R, RR>(this Either<L, R> either, Func<R, Either<L, RR>> f)
            // ReSharper disable once ConvertClosureToMethodGroup
            => either.Match(l => Left(l), f);

        public static Maybe<R> ToMaybe<L, R>(this Either<L, R> self)
            => self.Match(_ => Nothing, r => Just(r));

        //function application
        public static Either<L, RR> Apply<L, R, RR>(this Either<L, Func<R, RR>> @this, Either<L, R> arg)
            => @this.Match(
                 left: (errF) => Left(errF),
                 right: (f) => arg.Match<Either<L, RR>>(
                    right: (t) => Right(f(t)),
                    left: (err) => Left(err)));

        public static Either<L, Func<T2, R>> Apply<L, T1, T2, R>(this Either<L, Func<T1, T2, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, R>> Apply<L, T1, T2, T3, R>(this Either<L, Func<T1, T2, T3, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, T4, R>> Apply<L, T1, T2, T3, T4, R>(this Either<L, Func<T1, T2, T3, T4, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, T4, T5, R>> Apply<L, T1, T2, T3, T4, T5, R>(this Either<L, Func<T1, T2, T3, T4, T5, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, T4, T5, T6, R>> Apply<L, T1, T2, T3, T4, T5, T6, R>(this Either<L, Func<T1, T2, T3, T4, T5, T6, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, T4, T5, T6, T7, R>> Apply<L, T1, T2, T3, T4, T5, T6, T7, R>(this Either<L, Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);
    }
}
