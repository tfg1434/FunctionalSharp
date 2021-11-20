using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLibrary {
    public readonly partial struct Lst<T> : ICollection<T> {
        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T _) => throw ReadOnly();

        void ICollection<T>.Clear() => throw ReadOnly();

        public void CopyTo(T[] arr, int index) {
            if (arr is null) throw new ArgumentNullException(nameof(arr));
            if (index < 0 || index > arr.Length) throw new ArgumentOutOfRangeException(nameof(arr), index, "must be positive and less than or equal to array length");
            if (index + Count > arr.Length) throw new ArgumentException("destination array was not long enough", nameof(arr));

            int i = 0;
            for (Node? curr = _head; curr is not null; curr = curr.Next)
                arr[i++] = curr.Value;
        }

        public bool Contains(T value) {
            //var comparer = EqualityComparer<T>.Default;
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item) => throw ReadOnly();

        private static NotSupportedException ReadOnly() => new("Lst<T> is readonly");
    }
}
