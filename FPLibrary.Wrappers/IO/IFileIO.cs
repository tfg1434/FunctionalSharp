using System;
using FPLibrary;
using Unit = System.ValueTuple;

namespace FPLibrary.Wrappers.IO;

public interface IHasFile<E> where E : struct {
    IO<E, IFileIO> FileIO { get; }
}

public interface IFileIO {
    public Unit Copy(string from, string to, bool overwrite = false);
}