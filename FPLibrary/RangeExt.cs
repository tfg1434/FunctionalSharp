using System;
using System.Collections.Generic;

namespace FPLibrary {
    public static partial class F {
        public static IEnumerable<int> Range(int from, int? second = null, int? to = null)
            => RangeInt.Of(from, second, to);
    }
    
    public class RangeInt : Range<int> {
         private RangeInt(int from, int? to = null, int step = 1)
             : base(from, to, step, Comparer<int>.Default.Compare, static (x, y) => x + y) { }

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

    public class RangeChar : Range<char> {
        private static char Add(char x, char y) => unchecked((char) (x + y));
        private static char Subtract(char x, char y) => unchecked((char) (x - y));
        
        private RangeChar(char from, char? to = null, char step = (char)1)
            : base(from, to, step, Comparer<char>.Default.Compare, Add) { }

        internal static IEnumerable<char> Of(char from, char? second = null, char? to = null)
            => (second is null, to is null) switch {
                (false, false) => new RangeChar(from),
                (false, true) => new(from, to),
                (true, false) => new(from, step: Subtract(second!.Value, from)),
                (true, true) => new(from, to, Subtract(second!.Value, from)),
            };
    }
}