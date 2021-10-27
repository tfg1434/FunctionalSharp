using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public class Range<T> : IEnumerable<T> where T : struct {
        private readonly T _from;
        private readonly T? _to;
        private readonly T _step;
        private readonly bool _isAscending;
        private readonly bool _isInfinite;
        private readonly Func<T, T, bool> _isGtOrEqual;
        private readonly Func<T, T, T> _add;

        protected Range(T from, T? to, T step, bool isInfinite, bool isAscending, Func<T, T, bool> isGtOrEqual, Func<T, T, T> add) {
            _from = from;
            _to = to;
            _step = step;
            _isGtOrEqual = isGtOrEqual;
            _add = add;
            _isAscending = isAscending;
            _isInfinite = isInfinite;
        }

        public IEnumerable<T> AsEnumerable() {
            if (_isInfinite) {
                for (T x = _from;; x = _add(x, _step))
                    yield return x;
            } else {
                T x = _from;
                T y = _from;
                var isGtOrEqual = _isAscending ? _isGtOrEqual : _isGtOrEqual.Flip();

                while (true) {
                    //if (x < y            || x > to)
                    if (!isGtOrEqual(x, y) || !isGtOrEqual(_to!.Value, x))
                        yield break;
                    
                    yield return x;
                    
                    y = x;
                    
                    x = _add(x, _step);
                }
            }
        }
        
        public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
    }
}