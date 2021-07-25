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
        private readonly L? left;
        private readonly R? right;

        private readonly bool isRight;
        private bool isLeft => !isRight;

        internal Either(L l)
            => (isRight, left, right)
                = (false, l ?? throw new ArgumentNullException(nameof(l)), default);

        internal Either(R r)
            => (isRight, left, right)
                = (true, default, r ?? throw new ArgumentNullException(nameof(r)));

        public static implicit operator Either<L, R>(L left) => new(left);
        public static implicit operator Either<L, R>(R right) => new(right);
        public static implicit operator Either<L, R>(Either.Left<L> left) => new(left.Value);
        public static implicit operator Either<L, R>(Either.Right<R> right) => new(right.Value);

        public TR Match<TR>(Func<L, TR> l, Func<R, TR> r)
            => isRight ? r(right!) : l(left!);

        public Unit Match(Action<L> left, Action<R> right) 
            => Match(left.ToFunc(), right.ToFunc());

        public IEnumerable<R> AsEnumerable() {
            if (isRight) yield return right;
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
            => self.Match(_ => Nothing, Just);

        //function application
        public static Either<L, RR> Apply<L, R, RR>(this Either<L, Func<R, RR>> @this, Either<L, R> arg)
            => @this.Match(
                l: (errF) => Left(errF),
                r: (f) => arg.Match<Either<L, RR>>(
                    l: (err) => Left(err),
                    r: (t) => Right(f(t))));

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

        //query syntax
        public static Either<L, RR> Select<L, R, RR>(this Either<L, R> self, Func<R, RR> f)
            => self.Map(f);

        public static Either<L, RRR> SelectMany<L, R, RR, RRR>(this Either<L, R> self, Func<R, Either<L, RR>> bind, Func<R, RR, RRR> proj)
            => self.Match(
                l => Left(l),
                t => bind(t).Match<Either<L, RRR>>(
                    l => Left(l),
                    r => proj(t, r)));
    }
}
