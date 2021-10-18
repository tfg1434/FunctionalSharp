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
        private readonly T _empty;
        private readonly Func<T, T, int> _compare;
        private readonly Func<T, T, T> _add;

        private bool isInfinite => _to is null;

        protected Range(T from, T? to, T step, T empty, Func<T, T, int> compare, Func<T, T, T> add) {
            _from = from;
            _to = to;
            _step = step;
            _compare = compare;
            _add = add;
            _empty = empty;
            _isAscending = _compare(_step, _empty) > 0;
        }

        public IEnumerable<T> AsEnumerable() {
            if (_isAscending)
                for (T x = _from; isInfinite || _compare(x, _to!.Value) <= 0; x = _add(x, _step))
                    yield return x;
            else
                for (T x = _from; isInfinite || _compare(x, _to!.Value) >= 0; x = _add(x, _step))
                    yield return x;
        }
        
        public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
    }
}