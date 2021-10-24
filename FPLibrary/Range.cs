using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public class Range<T> : IEnumerable<T> where T : struct {
        private readonly T _from;
        private readonly T _to;
        private readonly T _step;
        private readonly bool _isAscending;
        private readonly bool _isInfinite;
        private readonly Func<T, T, bool> _isGtOrEqual;
        private readonly Func<T, T, T> _add;

        protected Range(T from, T to, T step, bool isInfinite, Func<T, T, bool> isGtOrEqual, Func<T, T, T> add) {
            _from = from;
            _to = to;
            _step = step;
            _isGtOrEqual = isGtOrEqual;
            _add = add;
            _isAscending = _isGtOrEqual(step, default);
            _isInfinite = isInfinite;
        }

        public IEnumerable<T> AsEnumerable() {
            if (_isInfinite) {
                for (T x = _from;; x = _add(x, _step))
                    yield return x;
            } else {
                T x = _from;
                T add = _from;
                
                while (_isAscending ? _isGtOrEqual(add = _add(add, _step), x) : _isGtOrEqual(x, add = _add(add, _step))) {
                    yield return x;

                    x = add;
                }
            }
            
            /*
            x <- from
            prev <- compare x to
            -- second bigger than first = negative comp
            
            loop {
                comp <- compare x to
                
                if (_isAscending)
                    advance = comp <= 0 && prev == comp
                else
                    advance = comp >= 0 && prev == comp
                
                if (!advance) break;
                
                yield x
                
                x <- add x step
                prev <- comp
            }
            */
        }
        
        public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
    }
}