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
        private readonly Func<T, T, int> _compare;
        private readonly Func<T, T, T> _add;

        protected Range(T from, T to, T step, Func<T, T, int> compare, Func<T, T, T> add, int? count = null) {
            _from = from;
            _to = to;
            _step = step;
            _compare = compare;
            _add = add;
            _isAscending = _compare(_step, default) > 0;
            _isInfinite = to is null;
        }

        public IEnumerable<T> AsEnumerable() {
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

            T x = _from;
            int prevComp = _compare(x, );
            
            
        }
        
        public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
    }
}