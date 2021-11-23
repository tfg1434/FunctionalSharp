using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public readonly partial struct Lst<T> {
    /// <summary>
    /// Match the two states of the list
    /// </summary>
    /// <param name="empty">Function to run if the list is empty</param>
    /// <param name="cons">Ternary function to run if the list is cons</param>
    /// <typeparam name="R">Return type of functions</typeparam>
    [Pure]
    public R Match<R>(Func<R> empty, Func<T, Lst<T>, R> cons)
        => IsEmpty ? empty() : cons(Head(), Tail());

    /// <summary>
    /// See if a value exists in the list
    /// </summary>
    /// <param name="value">Value to search for</param>
    /// <param name="comparer">Comparer to use while searching. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <returns>Whether <paramref name="value"/> exists in the list</returns>
    [Pure]
    public bool Contains(T value, IEqualityComparer<T>? comparer = null) {
        comparer ??= EqualityComparer<T>.Default;
        
        for (Node? curr = _head; curr is not null; curr = curr.Next)
            if (comparer.Equals(curr.Value, value)) return true;
        
        return false;
    }

    /// <summary>
    /// Equate this list to another list quickly
    /// </summary>
    /// <param name="other"><see cref="Lst{T}"/> to equate to</param>
    /// <param name="comparer">Comparer to use. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <returns>Whether the two lists are equal.</returns>
    [Pure]
    public bool SequenceEqual(Lst<T> other, IEqualityComparer<T>? comparer = null) {
        if (_count != other._count) return false;

        comparer ??= EqualityComparer<T>.Default;

        Node? selfCurr = _head;
        Node? otherCurr = other._head;

        while (true) {
            //ref equals
            if (selfCurr == otherCurr) return true;

            //we pre-validated counts
            if (selfCurr is null || !comparer.Equals(selfCurr.Value, otherCurr!.Value)) 
                return false;

            selfCurr = selfCurr.Next;
            otherCurr = otherCurr.Next;
        }
    }
    
    #region Prepend
    
    /// <summary>
    /// Prepend an item to the front of the list
    /// </summary>
    /// <param name="item">Item to prepend</param>
    /// <returns>New list with <paramref name="item"/> prepended</returns>
    [Pure]
    public Lst<T> Prepend(T item) 
        => new(new(item) { Next = _head }, _count + 1);

    /// <summary>
    /// Prepend an enumerable of items to the front of the list
    /// </summary>
    /// <param name="items">Items to prepend</param>
    /// <returns>New list with <paramref name="items"/> prepended</returns>
    [Pure]
    public Lst<T> Prepend(IEnumerable<T> items) {
        if (_count == 0) return Of(items);
        if (items is Lst<T> list) return Prepend(list);

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

    /// <summary>
    /// Prepend a <see cref="Lst{T}"/> to the front of the list
    /// </summary>
    /// <param name="list"><see cref="Lst{T}"/> to prepend</param>
    /// <returns>New list with <paramref name="list"/> prepended</returns>
    [Pure]
    public Lst<T> Prepend(Lst<T> list) {
        if (list._count == 0) return this;
        if (_count == 0) return list;

        (Node newHead, Node newLast) = CopyNonEmptyRange(list._head!, null);
        newLast.Next = _head;

        return new Lst<T>(newHead, _count + list._count);
    }
    
    #endregion

    #region Append

    /// <summary>
    /// Append an item to the end of the list
    /// </summary>
    /// <param name="item">Item to append</param>
    /// <returns>New list with <paramref name="item"/> appended</returns>
    [Pure]
    public Lst<T> Append(T item) {
        if (_count == 0) return Of(item);

        (Node newHead, Node newLast) = CopyNonEmptyRange(_head!, null);
        newLast.Next = new(item);

        return new(newHead, _count + 1);
    }

    /// <summary>
    /// Append an enumerable of items to the end of the list
    /// </summary>
    /// <param name="items">Items to append</param>
    /// <returns>New list with <paramref name="items"/> appended</returns>
    [Pure]
    public Lst<T> Append(IEnumerable<T> items) => Append(Of(items));

    /// <summary>
    /// Append a <see cref="Lst{T}"/> to the end of the list
    /// </summary>
    /// <param name="list"><see cref="Lst{T}"/> to append</param>
    /// <returns>New list with <paramref name="list"/> appended</returns>
    [Pure]
    public Lst<T> Append(Lst<T> list) => list.Prepend(this);

    #endregion
    
    #region Remove

    /// <summary>
    /// Remove an item from the list
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <param name="comparer">Comparer to use. If null, uses <see cref="EqualityComparer{T}"/></param>
    /// <returns></returns>
    [Pure]
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
    
    /// <summary>
    /// Remove an item from the list at a specific index
    /// </summary>
    /// <param name="index">Index to remove at</param>
    /// <returns>New list with index removed</returns>
    [Pure]
    public Lst<T> RemoveAt(int index) 
        => RemoveAtRet(index).List;

    /// <summary>
    /// Remove an item from the list at a specific index and return it
    /// </summary>
    /// <param name="index">Index to remove at</param>
    /// <returns>New list with index removed and removed item</returns>
    [Pure]
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

    /// <summary>
    /// Remove all items from the list that match a predicate
    /// </summary>
    /// <param name="pred">Predicate to remove with</param>
    /// <returns>New list with items removed</returns>
    [Pure]
    public Lst<T> RemoveAll(Func<T, bool> pred) {
        //remove prefix as it doesn't require copying
        var noPrefix = SkipWhile(pred);

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

    /// <summary>
    /// Skip the first <paramref name="count"/> items
    /// </summary>
    /// <param name="count">Number of items to skip</param>
    /// <returns>New list with items skipped</returns>
    [Pure]
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

    /// <summary>
    /// Skip the first items that match a predicate
    /// </summary>
    /// <param name="pred">Predicate to use</param>
    /// <returns>New list with items skipped</returns>
    [Pure]
    public Lst<T> SkipWhile(Func<T, bool> pred) {
        Node? newHead = _head;
        int count = 0;
        
        while (newHead is not null && pred(newHead.Value)) {
            count++;
            newHead = newHead.Next;
        }

        return new(newHead, _count - count);
    }
    
    /// <summary>
    /// Skip the first items that match a ternary predicate
    /// </summary>
    /// <param name="pred">Predicate to use</param>
    /// <returns>New list with items skipped</returns>
    [Pure]
    public Lst<T> SkipWhile(Func<T, int, bool> pred) {
        Node? newHead = _head;
        int index = 0;
        int count = 0;
        
        while (newHead is not null && pred(newHead.Value, index)) {
            count++;
            index++;
            newHead = newHead.Next;
        }

        return new(newHead, _count - count);
    }
    
    #endregion

    /// <summary>
    /// Slice the list
    /// </summary>
    /// <param name="index">Index to start slicing from</param>
    /// <param name="count">Number of items to slice</param>
    /// <returns>Sliced section of list</returns>
    /// <exception cref="ArgumentOutOfRangeException">If index or count is out of range</exception>
    [Pure]
    public Lst<T> Slice(int index, int count) {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (index + count > _count) throw new ArgumentOutOfRangeException($"{nameof(index)} {nameof(count)}");

        if (count == 0) return Empty;

        Lst<T> skipped = Skip(index);

        if (count == skipped.Count) return skipped;

        Node? curr = skipped._head!.Next;
        for (int i = 1; i < count; i++) curr = curr!.Next;
        (Node head, _) = CopyNonEmptyRange(skipped._head, curr);

        return new(head, count);
    }
}

