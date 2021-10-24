using System;
using System.Collections.Generic;

namespace FPLibrary; 

public static partial class F {
    public static IEnumerable<int> Range(int from, int? second = null, int to = int.MaxValue)
        => IntRange.Of(from, second, to);
    
    public static IEnumerable<double> Range(double from, double? second = null, double to = double.MaxValue)
        => DoubleRange.Of(from, second, to);
        
    public static IEnumerable<char> Range(char from, char? second = null, char to = char.MaxValue)
        => CharRange.Of(from, second, to);
}
    
class IntRange : Range<int> {
    private IntRange(int from, int to, int step)
        : base(from, to, step, false, static (x, y) => x >= y, static (x, y) => x + y) { }

    internal static IEnumerable<int> Of(int from, int? second, int to) 
        => (second is not null) switch {
            //from | from, to
            false => new IntRange(from, to, 1),
            //from, second | from, second, to
            true => new(from, to, second!.Value - from),
        };
}

class DoubleRange : Range<double> {
    private DoubleRange(double from, double to, double step)
        : base(from, to, step, false, static (x, y) => x >= y, 
            static (x, y) => x + y) { }
    
    internal static IEnumerable<double> Of(double from, double? second, double to) 
        => (second is not null) switch {
            //from | from, to
            false => new DoubleRange(from, to, 1),
            //from, second | from, second, to
            true => new(from, to, second!.Value - from),
        };
}

class CharRange : Range<char> {
    private static char Add(char x, char y) => unchecked((char) (x + y));

    private CharRange(char from, char to, char step)
        : base(from, to, step, false, static (x, y) => x >= y, Add) { }

    //char is bounded
    internal static IEnumerable<char> Of(char from, char? second, char to)
        => (second is not null) switch {
            //from...
            false => new CharRange(from, to, (char) 1),
            //from, second...
            true => new(from, to, Add(second!.Value, from)),
        };
}

