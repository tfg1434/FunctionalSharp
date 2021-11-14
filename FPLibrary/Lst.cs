using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary; 

public static partial class F {
    public static Lst<T> List<T>(IEnumerable<T> items) => Lst<T>.Of(items);
        
    public static Lst<T> List<T>(params T[] items) => Lst<T>.Of(items);

    public static Lst<T> ToLst<T>(this IEnumerable<T> src) => List(src);
}

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ImmutableEnumerableDebuggerProxy<>))]
public readonly partial struct Lst<T> : IReadOnlyCollection<T>, IEquatable<Lst<T>> {
    private readonly Node? _head;
    private readonly int _count;

    private Lst(Node? head, int count) {
        _head = head;
        _count = count;
    }

    public static Lst<T> Of(IEnumerable<T> items) {
        if (items is Lst<T> list) return list;

        (Node? node, int count) = items
            .Reverse()
            .Aggregate<T, (Node Node, int Count)>(
                (default!, 0),
                (acc, t) => (new(t) { Next = acc.Node }, acc.Count + 1)
            );

        return new(node, count);
    }
        
    private bool IsEmpty => Count == 0;
    public int Count => _count;
    public static Lst<T> Empty => default;
        
    public T Head {
        get {
            if (IsEmpty) ThrowEmpty();

            return _head!.Value;
        }
    }

    public Maybe<T> HeadSafe => IsEmpty ? Nothing : _head!.Value;

    public Lst<T> Tail {
        get {
            if (IsEmpty) ThrowEmpty();

            return new(_head!.Next, _count - 1);
        }
    }

    public Maybe<Lst<T>> TailSafe => IsEmpty ? Nothing : new Lst<T>(_head!.Next, _count - 1);
    
    #region Operations
    
    public R Match<R>(Func<R> empty, Func<T, Lst<T>, R> cons)
        => IsEmpty ? empty() : cons(Head, Tail);

    public bool Contains(T value, IEqualityComparer<T>? comparer = null) {
        comparer ??= EqualityComparer<T>.Default;
        
        for (Node? curr = _head; curr is not null; curr = curr.Next)
            if (comparer.Equals(curr.Value, value)) return true;
        
        return false;
    }

    public Lst<T> Prepend(T value) => new(new(value) { Next = _head }, _count + 1);

    public Lst<T> Prepend(IEnumerable<T> values) {
        if (_count == 0) return Of(values);
        if (values is Lst<T> list) return Prepend(list);
        if (values is null) throw new ArgumentNullException(nameof(values));

        using var enumerator = values.GetEnumerator();

        if (!enumerator.MoveNext()) return this;
        
        Node head = new(enumerator.Current);
        Node last = head;
        int count = 1;
        
        while (enumerator.MoveNext()) {
            last = last.Next = new(enumerator.Current);
            count++;
        }
        
        last.Next = _head;
        
        return new(head, count + _count);
    }

    #endregion
    
    #region Equality
    
    public override bool Equals(object? obj)
        => obj is Lst<T> lst && Equals(lst);
        
    public bool Equals(Lst<T> other) 
        => Count == other.Count && this.SequenceEqual(other);

    public static bool operator ==(Lst<T> self, Lst<T> other)
        => self.Equals(other);

    public static bool operator !=(Lst<T> self, Lst<T> other)
        => !(self == other);

    //no caching, readonly struct :/
    public override int GetHashCode()
        => FNV1A.Hash(AsEnumerable());
    
    #endregion

    private static void ThrowEmpty() => throw new InvalidOperationException("Lst is empty");
}