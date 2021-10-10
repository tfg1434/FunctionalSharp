// using System;
//
// namespace FPLibrary {
//     public sealed partial class Map<K, V> : IEquatable<Map<K, V>> where K : notnull {
//         public static bool operator ==(Map<K, V> self, Map<K, V> other)
//             => self.Equals(other);
//
//         public static bool operator !=(Map<K, V> self, Map<K, V> other)
//             => !(self == other);
//
//         public override bool Equals(object? obj)
//             => obj is Map<K, V> map && Equals(map);
//
//         public bool Equals(Map<K, V>? other) {
//             if (other is null) return false;
//             if (ReferenceEquals(this, other)) return true;
//             if (Count != other.Count) return false;
//             //if (hashCode != 0 && other.hashCode != 0) return false;
//
//             using var iterThis = GetEnumerator();
//             using var iterOther = other.GetEnumerator();
//
//             for (int i = 0; i < Count; i++) {
//                 iterThis.MoveNext();
//                 iterOther.MoveNext();
//                 
//                 if (keyComparer.Compare(iterThis.Current.Key, iterOther.Current.Key) != 0) return false;
//                 if (!valComparer.Equals(iterThis.Current.Val, iterOther.Current.Val)) return false;
//             }
//
//             return true;
//         }
//     }
// }