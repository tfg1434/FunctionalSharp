using System;
using System.Collections;
using System.Collections.Generic;

namespace FPLibrary {
    public class Range<T> : IEnumerable<T> {
        public readonly T From;
        //null = infinite
        public readonly T? To;
        public readonly T Step;
        public readonly bool IsAscending;

        private readonly IComparer<T> _comparer = Comparer<T>.Default;
        private readonly Func<T, T, T> _plus;

        protected Range(T from, T? to, T step, IComparer<T> comparer, Func<T, T, T> plus) {
            From = from;
            To = to;
            Step = step;
            // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
            IsAscending = _comparer.Compare(step, default(T)) >= 0;
            _comparer = comparer;
            _plus = plus;
        }
        
        public bool Contains(T value) {
            T? from = _comparer.Compare(From, To) > 0 ? To : From;
            T? to = _comparer.Compare(From, To) > 0 ? From : To;

            return _comparer.Compare(value, from) >= 0 && _comparer.Compare(value, to) <= 0;
        }
        
        public IEnumerable<T> AsEnumerable() {
            if (IsAscending) {
                for (T x = From; To is null || _comparer.Compare(x, To) <= 0; x = _plus(x, Step)) {
                    yield return x;
                }
            } else {
                for (T x = From; To is null || _comparer.Compare(x, To) >= 0; x = _plus(x, Step)) {
                    yield return x;
                }
            }
        }

        public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
    }
}