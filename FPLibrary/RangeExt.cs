using System;
using System.Collections.Generic;

namespace FPLibrary; 

public static partial class F {
    public static IEnumerable<int> Range(int from, int? second = null, int? to = null)
        => IntRange.Of(from, second, to);
        
    public static IEnumerable<char> Range(char from, char? second = null, char to = char.MaxValue)
        => CharRange.Of(from, second, to);
    
    public static IEnumerable<double> Range(double from, double? second = null, double? to = null)
        => DoubleRange.Of(from, second, to);
}
    
class IntRange : Range<int> {
    private IntRange(int from, int? to = null, int step = 1)
        : base(from, to, step, Comparer<int>.Default.Compare, static (x, y) => x + y) { }

    internal static IEnumerable<int> Of(int from, int? second = null, int? to = null) 
        => (second is not null, to is not null) switch {
            //from
            (false, false) => new IntRange(from),
            //from, to
            (false, true) => new(from, to),
            //from, second
            (true, false) => new(from, step: second!.Value - from),
            //from, second, to
            (true, true) => new(from, to, second!.Value - from),
        };
}

class DoubleRange : Range<double> {
    private DoubleRange(double from, double? to = null, double step = 1)
        : base(from, to, step, Comparer<double>.Default.Compare, static (x, y) => x + y) { }
    
    internal static IEnumerable<double> Of(double from, double? second = null, double? to = null) 
        => (second is not null, to is not null) switch {
            //from
            (false, false) => new DoubleRange(from),
            //from, to
            (false, true) => new(from, to),
            //from, second
            (true, false) => new(from, step: second!.Value - from),
            //from, second, to
            (true, true) => new(from, to, second!.Value - from),
        };
}

class CharRange : Range<char> {
    private static char Add(char x, char y) => unchecked((char) (x + y));

    private CharRange(char from, char to = char.MaxValue, char step = (char)1)
        : base(from, to, step, Comparer<char>.Default.Compare, Add) { }

    //char is bounded
    internal static IEnumerable<char> Of(char from, char? second = null, char to = char.MaxValue)
        => (second is not null) switch {
            //from...
            false => new CharRange(from, to),
            //from, second...
            true => new(from, to, Add(second!.Value, from)),
        };
}

