using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLibrary {
    using static F;

    public static class DictionaryExt {
        public static Maybe<T> LookUp<K, T>(this IDictionary<K, T> dict, K key)
            => dict.TryGetValue(key, out T val) ? Just(val) : Nothing;
    }
}
