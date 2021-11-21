using System;
using System.Diagnostics.Contracts;

namespace FunctionalSharp {
    public delegate Exceptional<T> Try<T>();

    public static partial class F {
        public static Try<T> Try<T>(Func<T> f) => () => f();
    }

    /// <summary>
    /// Extension methods for Try monad
    /// </summary>
    public static class TryExt {
        /// <summary>
        /// Run the Try monad and wrap the result in an Exceptional
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Pure]
        public static Exceptional<T> Run<T>(this Try<T> self) {
            try {
                return self();
            } catch (Exception ex) {
                return ex;
            }
        }

        /// <summary>
        /// Monadic Bind
        /// </summary>
        /// <param name="self">Try monad</param>
        /// <param name="f">Bind function</param>
        /// <typeparam name="T">Type of wrapped value</typeparam>
        /// <typeparam name="R">Type of resulting value</typeparam>
        /// <returns>Try with f binded over</returns>
        [Pure]
        public static Try<R> Bind<T, R>(this Try<T> self, Func<T, Try<R>> f)
            => ()
                => self.Run()
                    .Match(ex => ex, t => f(t).Run());

        /// <summary>
        /// Functor Map
        /// </summary>
        /// <param name="self">Try monad</param>
        /// <param name="f">Function to map</param>
        /// <typeparam name="T">Type of wrapped value</typeparam>
        /// <typeparam name="R">Resulting type</typeparam>
        /// <returns>Try monad with f mapped over</returns>
        [Pure]
        public static Try<R> Map<T, R>(this Try<T> self, Func<T, R> f)
            => () => self.Run().Map(f);

        /// <summary>
        /// Functor Map
        /// Curries the first value of the ternary function, then maps it
        /// </summary>
        /// <param name="self">Try monad</param>
        /// <param name="f">Ternary function to map</param>
        /// <typeparam name="T1">Type of wrapped value</typeparam>
        /// <typeparam name="T2">Type of second param of ternary function</typeparam>
        /// <typeparam name="R">Resulting type</typeparam>
        /// <returns>Try monad with f mapped over</returns>
        [Pure]
        public static Try<Func<T2, R>> Map<T1, T2, R>(this Try<T1> self, Func<T1, T2, R> f)
            => self.Map(f.CurryFirst());

        /// <summary>
        /// Functor BiMap
        /// </summary>
        /// <param name="self">Try monad</param>
        /// <param name="succ">Function to map in success state</param>
        /// <param name="ex">Function to map in exception state</param>
        /// <typeparam name="T">Type of wrapped value</typeparam>
        /// <typeparam name="R">Resulting type</typeparam>
        /// <returns>Mapped Try monad</returns>
        [Pure]
        public static Try<R> BiMap<T, R>(this Try<T> self, Func<T, R> succ, Func<Exception, R> ex)
            => () => self.Run().Match(ex, succ);
            
        /// <summary>
        /// Functor Map
        /// </summary>
        /// <param name="self">Try monad</param>
        /// <param name="f">Function to map</param>
        /// <typeparam name="T">Type of wrapped value</typeparam>
        /// <typeparam name="R">Resulting type</typeparam>
        /// <returns>Try monad with f mapped over</returns>
        [Pure]
        public static Try<R> Select<T, R>(this Try<T> self, Func<T, R> f)
            => self.Map(f);

        /// <summary>
        /// Monadic Bind with a projection
        /// </summary>
        /// <param name="self">Try monad</param>
        /// <param name="bind">Monadic bind function</param>
        /// <param name="proj">Projection function</param>
        /// <typeparam name="T">Type of wrapped value</typeparam>
        /// <typeparam name="R">Bind resulting type</typeparam>
        /// <typeparam name="PR">Project resulting type</typeparam>
        /// <returns>Try monad binded and projected</returns>
        [Pure]
        public static Try<PR> SelectMany<T, R, PR>(this Try<T> self, Func<T, Try<R>> bind, Func<T, R, PR> proj)
            => self.Bind(x => bind(x).Map(y => proj(x, y)));
    }
}