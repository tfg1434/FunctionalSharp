using System.Collections;
using System.Collections.Generic;

namespace FunctionalSharp; 

class DictionaryEnumerator<K, V> : IDictionaryEnumerator where K : notnull {
    private readonly IEnumerator<(K Key, V Value)> _inner;

    internal DictionaryEnumerator(IEnumerator<(K Key, V Value)> inner) 
        => _inner = inner;

    public object Current => Entry;
    public DictionaryEntry Entry => new(_inner.Current.Key, _inner.Current.Value);
    public object Key => _inner.Current.Key;
    public object? Value => _inner.Current.Value;

    public bool MoveNext() => _inner.MoveNext();

    public void Reset() => _inner.Reset();
}