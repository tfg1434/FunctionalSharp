using System.Collections.Generic;

namespace FPLibrary {
    using static F;

    public static partial class F {
        public static Either.Left<L> Left<L>(L l) => new(l);

        public static Either.Right<R> Right<R>(R r) => new(r);
    }

    public readonly struct Either<L, R> {
        private readonly L? _left;
        private readonly R? _right;

        private readonly bool _isRight;
        
        internal Either(L l)
            => (_isRight, _left, _right)
                = (false, l ?? throw new ArgumentNullException(nameof(l)), default);
        
        internal Either(R r)
            => (_isRight, _left, _right)
                = (true, default, r ?? throw new ArgumentNullException(nameof(r)));

        public static implicit operator Either<L, R>(L left) => new(left);

        public static implicit operator Either<L, R>(R right) => new(right);

        public static implicit operator Either<L, R>(Either.Left<L> left) => new(left.Value);

        public static implicit operator Either<L, R>(Either.Right<R> right) => new(right.Value);

        public TR Match<TR>(Func<L, TR> l, Func<R, TR> r)
            => _isRight ? r(_right!) : l(_left!);

        public Unit Match(Action<L> l, Action<R> r)
            => Match(l.ToFunc(), r.ToFunc());

        public IEnumerable<R> AsEnumerable() {
            if (_isRight) yield return _right!;
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
        //called bifunctor or biMap
        public static Either<LL, RR> BiMap<L, LL, R, RR>(this Either<L, R> self, Func<L, LL> left, Func<R, RR> right)
            => self.Match<Either<LL, RR>>(l => Left(left(l)), r => Right(right(r)));

        public static Either<L, Unit> ForEach<L, R>(this Either<L, R> either, Action<R> act)
            => either.Map(act.ToFunc());

        public static Either<L, RR> Bind<L, R, RR>(this Either<L, R> either, Func<R, Either<L, RR>> f)
            // ReSharper disable once ConvertClosureToMethodGroup
            => either.Match(l => Left(l), f);

        public static Maybe<R> ToMaybe<L, R>(this Either<L, R> self)
            => self.Match(_ => Nothing, Just);

        //function application
        public static Either<L, RR> Apply<L, R, RR>(this Either<L, Func<R, RR>> self, Either<L, R> arg)
            => self.Match(
                // ReSharper disable once ConvertClosureToMethodGroup
                l => Left(l),
                f => arg.Match<Either<L, RR>>(
                    // ReSharper disable once ConvertClosureToMethodGroup
                    l => Left(l),
                    t => Right(f(t))));

        public static Either<L, Func<T2, R>> Apply<L, T1, T2, R>(this Either<L, Func<T1, T2, R>> self,
            Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, R>> Apply<L, T1, T2, T3, R>(this Either<L, Func<T1, T2, T3, R>> self,
            Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, T4, R>> Apply<L, T1, T2, T3, T4, R>(
            this Either<L, Func<T1, T2, T3, T4, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, T4, T5, R>> Apply<L, T1, T2, T3, T4, T5, R>(
            this Either<L, Func<T1, T2, T3, T4, T5, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, T4, T5, T6, R>> Apply<L, T1, T2, T3, T4, T5, T6, R>(
            this Either<L, Func<T1, T2, T3, T4, T5, T6, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        public static Either<L, Func<T2, T3, T4, T5, T6, T7, R>> Apply<L, T1, T2, T3, T4, T5, T6, T7, R>(
            this Either<L, Func<T1, T2, T3, T4, T5, T6, T7, R>> self, Either<L, T1> arg)
            => Apply(self.Map(F.CurryFirst), arg);

        //query syntax
        public static Either<L, RR> Select<L, R, RR>(this Either<L, R> self, Func<R, RR> f)
            => self.Map(f);

        public static Either<L, RRR> SelectMany<L, R, RR, RRR>(this Either<L, R> self, Func<R, Either<L, RR>> bind,
            Func<R, RR, RRR> proj)
            => self.Match(
                // ReSharper disable once ConvertClosureToMethodGroup
                l => Left(l),
                t => bind(t).Match<Either<L, RRR>>(
                    // ReSharper disable once ConvertClosureToMethodGroup
                    l => Left(l),
                    r => proj(t, r)));
    }
}