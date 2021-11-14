using System;
using System.Collections.Generic;

namespace FPLibrary;

public readonly partial struct Lst<T> : ICollection<T> {
    bool ICollection<T>.IsReadOnly => true;

    void ICollection<T>.Add(T item) => throw new NotSupportedException();

    void ICollection<T>.Clear() => throw new NotSupportedException();

    public void CopyTo(T[] array, int arrayIndex) {
        if (arrayIndex < 0 || arrayIndex > array.Length || arrayIndex + Count > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "out of range");
        
        int i = arrayIndex;
        for (Node? curr = _head; curr != null; curr = curr.Next)
            array[i++] = curr.Value;
    }

    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
}