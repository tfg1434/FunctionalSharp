using System.Collections.Generic;

namespace FunctionalSharp; 

//FNV-1a 32-bit hash
static class FNV1A {
    private const int offset_basis = unchecked((int) 2166136261);
    private const int prime = 16777619;
    
    public static int Hash<T>(IEnumerable<T> items) {
        unchecked {
            return items.Aggregate(offset_basis,
                (hash, item) => (hash ^ (item is null ? 0 : item.GetHashCode())) * prime);
        }
    }
}