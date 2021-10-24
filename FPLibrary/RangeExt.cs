using System;
using System.Collections.Generic;
using System.Numerics;
using static FPLibrary.RangeUtils;

namespace FPLibrary; 

public static partial class F {
    public static IEnumerable<int> Range(int from, int? second = null, int? to = null) 
        => IntRange.Of(from, second, to);

    public static IEnumerable<double> Range(double from, double? second = null, double? to = null) 
        => DoubleRange.Of(from, second, to);

    public static IEnumerable<char> Range(char from, char? second = null, char? to = null) 
        => CharRange.Of(from, second, to);

    public static IEnumerable<BigInteger> Range(BigInteger from, BigInteger? second = null, BigInteger? to = null)
        => BigIntRange.Of(from, second, to);
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
            //from | from, to
            false => new CharRange(from, to.Value, (char) 1, isAscending),
            //from, second | from, second, to
            true => new(from, to.Value, Add(second!.Value, (char) -from), isAscending),
        };
    }
}

class BigIntRange : Range<BigInteger> {
    private BigIntRange(BigInteger from, BigInteger? to, BigInteger step, bool isAscending)
        : base(from, null, step, to is null, isAscending, 
            static (x, y) => x >= y, static (x, y) => x + y) { }
    
    public static IEnumerable<BigInteger> Of(BigInteger from, BigInteger? second, BigInteger? to) {
        bool isAscending = (second, to) switch {
            (null, null) => true,
            ({ }, null) when second > from => true,
            (null, { }) or ({ }, { }) when to > from => true,
            (_, _) => throw new ArgumentException("wtf"),
        };
        //bool isAscending = IsAscending(from, second, to);

        return (second is not null) switch {
            //from...
            false => new BigIntRange(from, null, (char) 1, isAscending),
            //from, second...
            true => new(from, null, second!.Value - from, isAscending),
        };
    }
}

static class RangeUtils {
    public static bool IsAscending<T>(T from, T? second, T? to) where T : struct, IComparisonOperators<T, T>
        => (second, to) switch {
            (null, null) => true,
            ({ }, null) when second.Value > from => true,
            (null, { }) or ({ }, { }) when to > from => true,
            (_, _) => throw new ArgumentException("wtf"),
        };
}

