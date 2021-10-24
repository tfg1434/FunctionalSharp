using System;
using System.Collections.Generic;

namespace FPLibrary; 

public static partial class F {
    public static IEnumerable<int> Range(int from, int? second = null, int? to = null) 
        => IntRange.Of(from, second, to);

    public static IEnumerable<double> Range(double from, double? second = null, double? to = null) 
        => DoubleRange.Of(from, second, to);

    public static IEnumerable<char> Range(char from, char? second = null, char? to = null) 
        => CharRange.Of(from, second, to);
}
    
class IntRange : Range<int> {
    private IntRange(int from, int to, int step, bool isAscending)
        : base(from, to, step, false, isAscending, static (x, y) => x >= y, 
            static (x, y) => x + y) { }

    internal static IEnumerable<int> Of(int from, int? second, int? to = null) {
        bool isAscending = second is null || second > from;
        to ??= isAscending ? int.MaxValue : int.MinValue;
        
        return (second is not null) switch {
            //from | from, to
            false => new IntRange(from, to.Value, 1, isAscending),
            //from, second | from, second, to
            true => new(from, to.Value, second!.Value - from, isAscending),
        };
    }
}

class DoubleRange : Range<double> {
    private DoubleRange(double from, double to, double step, bool isAscending)
        : base(from, to, step, false, isAscending, static (x, y) => x >= y, 
            static (x, y) => x + y) { }
    
    internal static IEnumerable<double> Of(double from, double? second, double? to = null) {
        bool isAscending = second is null || second > from;
        to ??= isAscending ? double.MaxValue : double.MinValue;
        
        return (second is not null) switch {
            //from | from, to
            false => new DoubleRange(from, to.Value, 1, isAscending),
            //from, second | from, second, to
            true => new(from, to.Value, second!.Value - from, isAscending),
        };
    }
}

class CharRange : Range<char> {
    private static char Add(char x, char y) => unchecked((char) (x + y));

    private CharRange(char from, char to, char step, bool isAscending)
        : base(from, to, step, false, isAscending, static (x, y) => x >= y, Add) { }

    //char is bounded
    internal static IEnumerable<char> Of(char from, char? second, char? to = null) {
        bool isAscending = second is null || second > from;
        to ??= isAscending ? char.MaxValue : char.MinValue;
        
        return (second is not null) switch {
            //from...
            false => new CharRange(from, to.Value, (char) 1, isAscending),
            //from, second...
            true => new(from, to.Value, Add(second!.Value, (char) -from), isAscending),
        };
    }
}

