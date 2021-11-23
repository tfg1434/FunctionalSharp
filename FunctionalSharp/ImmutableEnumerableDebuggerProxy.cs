using System.Collections.Generic;
using System.Diagnostics;

namespace FunctionalSharp; 

/// <summary>
/// Only use with immutable enumerables, because it caches the contents of enumeration
/// </summary>
/// <typeparam name="T"></typeparam>
class ImmutableEnumerableDebuggerProxy<T> {
    private readonly IEnumerable<T> _enumerable;
    private T[]? _cachedContents;

    public ImmutableEnumerableDebuggerProxy(IEnumerable<T> enumerable)
        => _enumerable = enumerable;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Contents => _cachedContents ??= _enumerable.ToArray();
}

sealed class ImmutableMapDebuggerProxy<K, V> : ImmutableEnumerableDebuggerProxy<(K Key, V Value)> 
    where K : notnull {

    public ImmutableMapDebuggerProxy(IEnumerable<(K Key, V Value)> map) : base(map) { }
}