using System;
using System.Collections.Generic;
using System.Linq;

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

    public bool SequenceEqual(Lst<T> other, IEqualityComparer<T>? comparer = null) {
        if (_count != other._count) return false;

        comparer ??= EqualityComparer<T>.Default;

        Node? selfCurr = _head;
        Node? otherCurr = other._head;

        while (true) {
            //ref equals
            if (selfCurr == otherCurr) return true;

            //we pre-validated counts
            if (selfCurr is null || !comparer.Equals(selfCurr.Value, otherCurr!.Value)) return false;

            selfCurr = selfCurr.Next;
            otherCurr = otherCurr.Next;
        }
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
    
    #region Remove

    public Lst<T> Remove(T item, IEqualityComparer<T>? comparer = null) {
        comparer ??= EqualityComparer<T>.Default;

        for (Node? curr = _head; curr is not null; curr = curr.Next) {
            if (!comparer.Equals(curr.Value, item)) continue;

            if (curr == _head)
                return new(_head!.Next, _count - 1);
                
            //copy everything up to curr
            (Node newHead, Node newLast) = CopyNonEmptyRange(_head!, curr);
            //skip curr, and go to curr.Next instead
            newLast.Next = curr.Next;

            return new(newHead, _count - 1);
        }

        return this;
    }
    
    public Lst<T> RemoveAt(int index) => RemoveAtRet(index).List;

    public (Lst<T> List, T Removed) RemoveAtRet(int index) {
        if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
        
        if (index == 0)
            return (new(_head!.Next, _count - 1), _head!.Value);

        Node? curr = _head!.Next;
        for (int i = 1; i < index; i++) curr = curr!.Next;

        (Node newHead, Node newLast) = CopyNonEmptyRange(_head, curr);
        newLast.Next = curr!.Next;

        return (new(newHead, _count - 1), curr.Value);
    }

    public Lst<T> RemoveAll(Func<T, bool> pred) {
        //remove prefix as it doesn't require copying
        var noPrefix = this.SkipWhile(pred);

        if (noPrefix._count == 0) return Empty;
        
        Node newHead = noPrefix._head!;
        Node last = newHead;
        int count = 0;
        Node? curr = newHead.Next;

        while (curr is not null) {
            if (pred(curr.Value)) {
                if (count == 0)
                    (newHead, last) = CopyNonEmptyRange(newHead, curr);

                count++;
            } else if (count != 0)
                //if copied, make a new Node
                last = last.Next = new(curr.Value);
            else
                //if not copied, just skip
                last = curr;

            curr = curr.Next;
        }
        
        return new(newHead, noPrefix._count - count);
    }
    
    #endregion
    
    #region Skip

    public Lst<T> Skip(int count) {
        if (count > _count) return Empty;

        int skipped = 0;
        Node? curr = _head;

        while (curr is not null && skipped < count) {
            skipped++;
            curr = curr.Next;
        }

        return new(curr, _count - skipped);
    }

    public Lst<T> SkipWhile(Func<T, bool> pred) {
        Node? newHead = _head;
        int count = 0;
        
        while (newHead is not null && pred(newHead.Value)) {
            count++;
            newHead = newHead.Next;
        }

        return new(newHead, _count - count);
    }
    
    #endregion
}