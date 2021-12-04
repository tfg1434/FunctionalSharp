using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace FunctionalSharp; 

public static partial class F {
    /// <summary>
    /// Construct a list from a single item
    /// </summary>
    /// <param name="item">Item to construct list from</param>
    /// <returns><see cref="Lst{T}"/> with single element</returns>
    [Pure]
    public static Lst<T> List<T>(T item) => Lst<T>.Of(item);
    
    /// <summary>
    /// Construct a list from an enumerable of items
    /// </summary>
    /// <param name="items">Items to construct list from</param>
    /// <returns><see cref="Lst{T}"/> filled with <paramref name="items"/></returns>
    [Pure]
    public static Lst<T> List<T>(IEnumerable<T> items) => Lst<T>.Of(items);
        
    /// <summary>
    /// Construct a list from an params of items
    /// </summary>
    /// <param name="items">Items to construct list from</param>
    /// <returns><see cref="Lst{T}"/> filled with <paramref name="items"/></returns>
    [Pure]
    public static Lst<T> List<T>(params T[] items) => Lst<T>.Of(items);

    /// <summary>
    /// Construct a list from an enumerable of items, but as an extension method
    /// </summary>
    /// <param name="src"><see cref="IEnumerable{T}"/> to get items from</param>
    /// <typeparam name="T">Type of items in the enumerable</typeparam>
    /// <returns><see cref="Lst{T}"/> filled with <paramref name="src"/></returns>
    [Pure]
    public static Lst<T> ToLst<T>(this IEnumerable<T> src) => List(src);
}

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ImmutableEnumerableDebuggerProxy<>))]
public readonly partial struct Lst<T> : IReadOnlyCollection<T>, IEquatable<Lst<T>>, IList<T> {
    private readonly Node? _head;
    private readonly int _count;

    private Lst(Node? head, int count) {
        _head = head;
        _count = count;
    }

    /// <summary>
    /// Construct a list from a single item
    /// </summary>
    /// <param name="item">Item to construct list from</param>
    /// <returns><see cref="Lst{T}"/> with single element</returns>
    [Pure]
    public static Lst<T> Of(T item) 
        => new(new(item), 1);
    
    /// <summary>
    /// Construct a list from an enumerable of items
    /// </summary>
    /// <param name="items">Items to construct list from</param>
    /// <returns><see cref="Lst{T}"/> filled with <paramref name="items"/></returns>
    [Pure]
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
        
    [Pure]
    public T this[int index] {
        get {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            Node? curr = _head!.Next;
            for (int i = 1; i < index; i++) curr = curr!.Next;
            
            return curr!.Value;
        }
        set => throw new NotSupportedException();
    }

    [Pure]
    public static Lst<T> operator +(Lst<T> lhs, Lst<T> rhs) 
        => lhs.Append(rhs);

    [Pure]
    public static Lst<T> operator +(Lst<T> lhs, T rhs) 
        => lhs.Append(rhs);

    [Pure]
    public static Lst<T> operator +(T lhs, Lst<T> rhs) 
        => rhs.Prepend(lhs);

    private bool IsEmpty => Count == 0;
    
    /// <summary>
    /// Number of items in the list
    /// </summary>
    [Pure]
    public int Count => _count;
    
    /// <summary>
    /// The empty list
    /// </summary>
    [Pure]
    public static Lst<T> Empty => default;
    
    /// <summary>
    /// Head (first item) of the list. If list is empty, returns Nothing
    /// </summary>
    [Pure]
    public Maybe<T> HeadSafe => IsEmpty ? Nothing : _head!.Value;
    
    /// <summary>
    /// Tail (everything except head) of the list. If list is empty, returns Nothing
    /// </summary>
    [Pure]
    public Maybe<Lst<T>> TailSafe 
        => IsEmpty ? Nothing : new Lst<T>(_head!.Next, _count - 1);
    
    /// <summary>
    /// Head (first item) of the list.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the list is empty</exception>
    [Pure]
    public T Head() {
        if (IsEmpty) ThrowEmpty();

        return _head!.Value;
    }

    /// <summary>
    /// Tail (everything except head) of the list.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the list is empty</exception>
    [Pure]
    public Lst<T> Tail() {
        if (IsEmpty) ThrowEmpty();

        return new(_head!.Next, _count - 1);
    }

    #region Equality
    
    public override bool Equals(object? obj)
        => obj is Lst<T> lst && Equals(lst);
    
    public bool Equals(Lst<T> other) 
        => SequenceEqual(other);

    [Pure]
    public static bool operator ==(Lst<T> self, Lst<T> other)
        => self.Equals(other);

    [Pure]
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