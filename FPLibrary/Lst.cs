﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary; 

public static partial class F {
    public static Lst<T> List<T>(T item) => Lst<T>.Of(item);
    
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

    public static Lst<T> Of(T value) => new(new(value), 1);
    
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
        
    public T this[int index] {
        get {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            Node? curr = _head!.Next;
            for (int i = 1; i < index; i++) curr = curr!.Next;
            
            return curr!.Value;
        }
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

    #region Equality
    
    public override bool Equals(object? obj)
        => obj is Lst<T> lst && Equals(lst);
        
    public bool Equals(Lst<T> other) 
        => SequenceEqual(other);

    public static bool operator ==(Lst<T> self, Lst<T> other)
        => self.Equals(other);

    public static bool operator !=(Lst<T> self, Lst<T> other)
        => !(self == other);

    //no caching, readonly struct :/
    public override int GetHashCode()
        => FNV1A.Hash(AsEnumerable());
    
    #endregion

    private static void ThrowEmpty() => throw new InvalidOperationException("Lst is empty");

    private static (Node NewHead, Node NewLast) CopyNonEmptyRange(Node head, Node? last) {
        Node newHead;
        Node newLast = newHead = new(head.Value);
        
        for (Node? curr = head.Next; curr != last; curr = curr.Next)
            newLast = newLast.Next = new(curr!.Value);

        return (newHead, newLast);
    }
}