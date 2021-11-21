using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

/// <summary>
/// Range of values
/// </summary>
/// <typeparam name="T">Type of values</typeparam>
public class Range<T> : IEnumerable<T> where T : struct {
    private readonly T _from;
    private readonly T? _to;
    private readonly T _step;
    private readonly bool _isSecondAscending;
    private readonly bool _isInfinite;
    private readonly Func<T, T, bool> _isGtOrEqual;
    private readonly Func<T, T, T> _add;

    protected Range(T from, T? to, T step, bool isInfinite, bool isSecondAscending, Func<T, T, bool> isGtOrEqual, Func<T, T, T> add) {
        _from = from;
        _to = to;
        _step = step;
        _isGtOrEqual = isGtOrEqual;
        _add = add;
        _isSecondAscending = isSecondAscending;
        _isInfinite = isInfinite;
    }

    [Pure]
    public IEnumerable<T> AsEnumerable() {
        if (_isInfinite) {
            for (T x = _from;; x = _add(x, _step))
                yield return x;
        } else {
            T x = _from;
            T y = _from;
            var isGtOrEqual = _isSecondAscending ? _isGtOrEqual : _isGtOrEqual.Flip();

            while (true) {
                //if (x < y            || x > to)
                if (!isGtOrEqual(x, y) || !isGtOrEqual(_to!.Value, x))
                    yield break;
                    
                yield return x;
                    
                y = x;
                    
                x = _add(x, _step);
            }
        }
    }
        
    [Pure]
    public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();
        
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
}