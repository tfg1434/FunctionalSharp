using System;
using System.Collections.Generic;

namespace FPLibrary {
    public static partial class F {
        public static IEnumerable<int> Range(int from, int? second = null, int? to = null)
            => RangeInt.Of(from, second, to);
    }
    
    public class RangeInt : Range<int> {
         private RangeInt(int from, int? to = null, int step = 1)
             : base(from, to, step, 0, Comparer<int>.Default.Compare, (x, y) => x + y) { }

         internal static IEnumerable<int> Of(int from, int? second = null, int? to = null) 
             => (second is not null, to is not null) switch {
                 //from
                 (false, false) => new RangeInt(from),
                 //from, to
                 (false, true) => new(from, to),
                 //from, second
                 (true, false) => new(from, step: second!.Value - from),
                 //from, second, to
                 (true, true) => new(from, to, second!.Value - from),
             };
    }
}