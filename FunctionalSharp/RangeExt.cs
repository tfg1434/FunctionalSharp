using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using static FunctionalSharp.RangeUtils;

namespace FunctionalSharp; 

public static partial class F {
    /// <summary>
    /// Construct an int Range
    /// </summary>
    /// <remarks>
    /// Same syntax as Haskell's range. [from,second..to] https://riptutorial.com/haskell/example/9516/ranges
    /// Supports infinite ranges
    /// </remarks>
    [Pure]
    public static IEnumerable<int> Range(int from, int? second = null, int? to = null) 
        => IntRange.Of(from, second, to);

    /// <summary>
    /// Construct a double Range
    /// </summary>
    /// <remarks>
    /// Same syntax as Haskell's range. [from,second..to] https://riptutorial.com/haskell/example/9516/ranges
    /// Supports infinite ranges
    /// </remarks>
    [Pure]
    public static IEnumerable<double> Range(double from, double? second = null, double? to = null) 
        => DoubleRange.Of(from, second, to);

    /// <summary>
    /// Construct a char Range
    /// </summary>
    /// <remarks>
    /// Same syntax as Haskell's range. [from,second..to] https://riptutorial.com/haskell/example/9516/ranges
    /// Supports infinite ranges
    /// </remarks>
    [Pure]
    public static IEnumerable<char> Range(char from, char? second = null, char? to = null) 
        => CharRange.Of(from, second, to);

    /// <summary>
    /// Construct a BigInt Range
    /// </summary>
    /// <remarks>
    /// Same syntax as Haskell's range. [from,second..to] https://riptutorial.com/haskell/example/9516/ranges
    /// Supports infinite ranges
    /// </remarks>
    [Pure]
    public static IEnumerable<BigInteger> Range(BigInteger from, BigInteger? second = null, BigInteger? to = null)
        => BigIntRange.Of(from, second, to);
}

class IntRange : Range<int> {
    private IntRange(int from, int to, int step, bool isSecondAscending)
        : base(from, to, step, false, isSecondAscending, static (x, y) => x >= y, 
            static (x, y) => x + y) { }

    internal static IEnumerable<int> Of(int from, int? second, int? to = null) {
        bool isSecondAscending = IsSecondAscending(from, second);
        to ??= isSecondAscending ? int.MaxValue : int.MinValue;
        
        return second switch {
            //from | from, to
            null => new IntRange(from, to.Value, 1, isSecondAscending),
            //from, second | from, second, to
            { } => new(from, to.Value, second!.Value - from, isSecondAscending),
        };
    }
}

class DoubleRange : Range<double> {
    private DoubleRange(double from, double to, double step, bool isSecondAscending)
        : base(from, to, step, false, isSecondAscending, static (x, y) => x >= y, 
            static (x, y) => x + y) { }
    
    internal static IEnumerable<double> Of(double from, double? second, double? to = null) {
        bool isSecondAscending = IsSecondAscending(from, second);
        to ??= isSecondAscending ? double.MaxValue : double.MinValue;
        
        return second switch {
            //from | from, to
            null => new DoubleRange(from, to.Value, 1, isSecondAscending),
            //from, second | from, second, to
            { } => new(from, to.Value, second!.Value - from, isSecondAscending),
        };
    }
}

class CharRange : Range<char> {
    private static char Add(char x, char y) => unchecked((char) (x + y));

    private CharRange(char from, char to, char step, bool isSecondAscending)
        : base(from, to, step, false, isSecondAscending, static (x, y) => x >= y, Add) { }

    //char is bounded
    internal static IEnumerable<char> Of(char from, char? second, char? to = null) {
        bool isSecondAscending = IsSecondAscending(from, second);
        to ??= isSecondAscending ? char.MaxValue : char.MinValue;
        
        return second switch {
            //from | from, to
            null => new CharRange(from, to.Value, (char) 1, isSecondAscending),
            //from, second | from, second, to
            { } => new(from, to.Value, Add(second!.Value, (char) -from), isSecondAscending),
        };
    }
}

class BigIntRange : Range<BigInteger> {
    private BigIntRange(BigInteger from, BigInteger? to, BigInteger step, bool isSecondAscending)
        : base(from, to, step, to is null, isSecondAscending, 
            static (x, y) => x >= y, static (x, y) => x + y) { }
    
    internal static IEnumerable<BigInteger> Of(BigInteger from, BigInteger? second, BigInteger? to) {
        bool isSecondAscending = IsSecondAscending(static (x, y) => x > y, from, second);

        return second switch {
            //from | from, to
            null => new BigIntRange(from, to, (char) 1, isSecondAscending),
            //from, second | from, second, to
            { } => new(from, to, second.Value - from, isSecondAscending),
        };
    }
}

static class RangeUtils {
    public static bool IsSecondAscending<T>(T from, T? second) where T : struct, IComparisonOperators<T, T>
        => second is null || second > from;
    
    public static bool IsSecondAscending<T>(Func<T, T, bool> isGt, T from, T? second) where T : struct
        => second is null || isGt(second.Value, from);
}

