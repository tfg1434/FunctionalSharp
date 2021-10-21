﻿using System;
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
        private readonly bool _prev = false; //for use with bounded types
        private readonly Func<T, T, int> _compare;
        private readonly Func<T, T, T> _add;

        protected Range(T from, T? to, T step, Func<T, T, int> compare, Func<T, T, T> add) {
            _from = from;
            _to = to;
            _step = step;
            _compare = compare;
            _add = add;
            _isAscending = _compare(_step, default) > 0;
            _isInfinite = to is null;
        }

        public IEnumerable<T> AsEnumerable() {
            T x = _from;
            
            while (true) {
                int comp = _compare(x, _to!.Value);
                bool advance = _isAscending ? comp <= 0 : comp >= 0;
                
                
            }

            // if (_isAscending)
            //     for (T x = _from; _isInfinite || _compare(x, _to!.Value) <= 0; x = _add(x, _step)) {
            //         if (prev) {
            //             int breakpoint = 10;
            //         }
            //         
            //         if (x.Equals(_to!.Value)) {
            //             prev = true;
            //         }
            //
            //         yield return x;
            //     }
            //         
            // else
            //     for (T x = _from; _isInfinite || _compare(x, _to!.Value) >= 0; x = _add(x, _step))
            //         yield return x;
        }
        
        public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
    }
}