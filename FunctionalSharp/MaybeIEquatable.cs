using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public readonly partial struct Maybe<T> : IEquatable<NothingType>, IEquatable<Maybe<T>> {
    public bool Equals(Maybe<T> other)
        => IsJust == other.IsJust && (IsNothing || _value!.Equals(other._value));

    public bool Equals(NothingType _) => !IsJust;

    public override bool Equals(object? other)
        => other is Maybe<T> m && Equals(m);

    public override int GetHashCode()
        => IsJust ? _value!.GetHashCode() : 0;

    [Pure]
    public static bool operator ==(Maybe<T> self, Maybe<T> other) => self.Equals(other);

    [Pure]
    public static bool operator !=(Maybe<T> self, Maybe<T> other) => !(self == other);
}