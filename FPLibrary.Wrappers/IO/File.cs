using Unit = System.ValueTuple;
using FPLibrary;

namespace FPLibrary.Wrappers.IO;

public static class File<E> where E : struct, IHasFile<E> {
    public static IO<E, Unit> Copy(string from, string to, bool overwrite = false)
        => default(E).FileIO.Map(e => e.Copy(from, to, overwrite));
    
    
}