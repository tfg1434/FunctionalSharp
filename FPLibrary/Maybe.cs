using System.Collections.Generic;

namespace FPLibrary; 

public static partial class F {
    public static NothingType Nothing => default;

    public static Maybe<T> Just<T>(T value)
        => new(value);

    public static Maybe<T> Jull<T>(T? value)
        => value is null ? Nothing : new Maybe<T>(value);

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

    public bool IsNothing => !IsJust;
    public bool IsJust { get; }

    internal Maybe(T t) {
        IsJust = true;
        _value = t;
    }

    public static implicit operator Maybe<T>(NothingType _) => default;

    public static implicit operator Maybe<T>(T? t)
        => t is null ? Nothing : Just(t);

    public R Match<R>(Func<R> nothing, Func<T, R> just)
        => IsJust ? just(_value!) : nothing();

    public IEnumerable<T> AsEnumerable() {
        if (IsJust) yield return _value!;
    }

    public override string ToString() => IsJust ? $"Just({_value})" : "Nothing";
}