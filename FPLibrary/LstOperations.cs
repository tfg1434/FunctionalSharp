using System;
using System.Collections.Generic;

namespace FPLibrary; 

public readonly partial struct Lst<T> {
    public R Match<R>(Func<R> empty, Func<T, Lst<T>, R> cons)
        => IsEmpty ? empty() : cons(Head, Tail);

    public bool Contains(T value, IEqualityComparer<T>? comparer = null) {
        comparer ??= EqualityComparer<T>.Default;
        
        for (Node? curr = _head; curr is not null; curr = curr.Next)
            if (comparer.Equals(curr.Value, value)) return true;
        
        return false;
    }

    #region Prepend
    
    public Lst<T> Prepend(T item) => new(new(item) { Next = _head }, _count + 1);

    public Lst<T> Prepend(IEnumerable<T> items) {
        if (_count == 0) return Of(items);
        if (items is Lst<T> list) return Prepend(list);
        if (items is null) throw new ArgumentNullException(nameof(items));

        using var enumerator = items.GetEnumerator();

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

    public Lst<T> Prepend(Lst<T> list) {
        if (list._count == 0) return this;
        if (_count == 0) return list;

        (Node newHead, Node newLast) = CopyNonEmptyRange(list._head!, null);
        newLast.Next = _head;

        return new Lst<T>(newHead, _count + list._count);
    }
    
    #endregion

    #region Add

    public Lst<T> Add(T item) {
        if (_count == 0) return Of(item);

        (Node newHead, Node newLast) = CopyNonEmptyRange(_head!, null);
        newLast.Next = new(item);

        return new(newHead, _count + 1);
    }

    public Lst<T> Add(IEnumerable<T> items) => Add(Of(items));

    public Lst<T> Add(Lst<T> list) => list.Prepend(this);

    #endregion
}