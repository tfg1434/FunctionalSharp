using System.Collections.Generic;

namespace FunctionalSharp; 

//FNV-1a 32-bit hash
static class FNV1A {
    public const int OffsetBasis = unchecked((int) 2166136261);
    public const int Prime = 16777619;

    //TODO: null deref?
    public static int Hash<T>(IEnumerable<T> items) {
        unchecked {
            return items.Aggregate(OffsetBasis,
                (hash, item) => (hash ^ item.GetHashCode()) * 16777619);
        }
    }
}