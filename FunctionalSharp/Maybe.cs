using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public static partial class F {
    /// <summary>
    /// Ctor for Maybe in Nothing state
    /// </summary>
    [Pure]
    public static NothingType Nothing => default;

    /// <summary>
    /// Ctor for Maybe in Just state
    /// </summary>
    /// <param name="value">Wrapped value</param>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    [Pure]
    public static Maybe<T> Just<T>(T value)
        => new(value);

    /// <summary>
    /// Maybe nullable utility
    /// </summary>
    /// <param name="value">Nullable value</param>
    /// <typeparam name="T">Type of wrapped value</typeparam>
    /// <remarks>
    /// This is a convenience method for creating a Maybe from a nullable value.
    /// If the value is null, the Maybe will be in a Nothing state. Otherwise, it will be in a Just state.
    /// </remarks>
    [Pure]
    public static Maybe<T> Jull<T>(T? value)
        => value is null ? Nothing : new Maybe<T>(value);

    /// <summary>
    /// Safe cast function
    /// </summary>
    /// <param name="value">Value to cast</param>
    /// <typeparam name="R">Type to cast to</typeparam>
    /// <returns>Wrapped value casted to R</returns>
    [Pure]
    public static Maybe<R> Cast<R>(in object value) {
        try {
            return (R) value;
        } catch {
            return Nothing;
        }
    }
}

public readonly struct NothingType { }

public readonly partial struct Maybe<T> {
    private readonly T? _value;

    /// <summary>
    /// Is the Maybe in a Nothing state
    /// </summary>
    [Pure]
    public bool IsNothing => !IsJust;
    
    /// <summary>
    /// Is the Maybe in a Just state
    /// </summary>
    [Pure]
    public bool IsJust { get; }

    internal Maybe(T t) {
        IsJust = true;
        _value = t;
    }

    [Pure]
    public static implicit operator Maybe<T>(NothingType _) => default;

    [Pure]
    public static implicit operator Maybe<T>(T? t)
        => t is null ? Nothing : Just(t);

    /// <summary>
    /// Match the two states of Maybe
    /// </summary>
    /// <param name="nothing">Function to run in Nothing state</param>
    /// <param name="just">Function to run in Just state</param>
    /// <typeparam name="R">Return type of functions</typeparam>
    [Pure]
    public R Match<R>(Func<R> nothing, Func<T, R> just)
        => IsJust ? just(_value!) : nothing();

    /// <summary>
    /// Convert the Maybe to an <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <remarks>
    /// If the Maybe is in a Just state, the value is wrapped in an <see cref="IEnumerable{T}"/> of length 1
    /// Otherwise, an empty IEnumerable is returned
    /// </remarks>
    [Pure]
    public IEnumerable<T> AsEnumerable() {
        if (IsJust) yield return _value!;
    }

    [Pure]
    public override string ToString() => IsJust ? $"Just({_value})" : "Nothing";
}