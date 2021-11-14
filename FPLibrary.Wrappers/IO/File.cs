using Unit = System.ValueTuple;
using FPLibrary;

namespace FPLibrary.Wrappers.IO;

public static class File<E> where E : struct, IHasFile<E> {
    // private static IO<E, R> MapEnv<R>(Func<IFileIO, R> f)
    //     => default(E).FileIO.Map(f);
    
    public static IO<E, Unit> AppendAllLines(string path, IEnumerable<string> contents)
        => default(E).FileIO.Map(env => env.AppendAllLines(path, contents));

    public static IO<E, Unit> AppendAllText(string path, string contents)
        => default(E).FileIO.Map(env => env.AppendAllText(path, contents));

    public static IO<E, Unit> Copy(string from, string to, bool overwrite = false)
        => default(E).FileIO.Map(env => env.Copy(from, to, overwrite));
    
    public static IO<E, FileStream> Create(string path)
        => default(E).FileIO.Map(env => env.Create(path));
    
    public static IO<E, StreamWriter> CreateText(string path)
        => default(E).FileIO.Map(env => env.CreateText(path));
    
    public static IO<E, Unit> Delete(string path)
        => default(E).FileIO.Map(env => env.Delete(path));
    
    public static IO<E, bool> Exists(string? path)
        => default(E).FileIO.Map(env => env.Exists(path));
    
    public static IO<E, FileStream> Open(string path, FileMode mode)
        => default(E).FileIO.Map(env => env.Open(path, mode));
    
    public static IO<E, FileStream> OpenRead(string path)
        => default(E).FileIO.Map(env => env.OpenRead(path));
    
    public static IO<E, StreamReader> OpenText(string path)
        => default(E).FileIO.Map(env => env.OpenText(path));
    
    public static IO<E, FileStream> OpenWrite(string path)
        => default(E).FileIO.Map(env => env.OpenWrite(path));
    
    public static IO<E, byte[]> ReadAllBytes(string path)
        => default(E).FileIO.Map(env => env.ReadAllBytes(path));
    
    public static IO<E, string[]> ReadAllLines(string path)
        => default(E).FileIO.Map(env => env.ReadAllLines(path));
    
    public static IO<E, string> ReadAllText(string path)
        => default(E).FileIO.Map(env => env.ReadAllText(path));
    
    public static IO<E, IEnumerable<string>> ReadLines(string path)
        => default(E).FileIO.Map(env => env.ReadLines(path));
    
    public static IO<E, Unit> WriteAllBytes(string path, byte[] bytes)
        => default(E).FileIO.Map(env => env.WriteAllBytes(path, bytes));
    
    public static IO<E, Unit> WriteAllLines(string path, IEnumerable<string> lines)
        => default(E).FileIO.Map(env => env.WriteAllLines(path, lines));
    
    public static IO<E, Unit> WriteAllText(string path, string contents)
        => default(E).FileIO.Map(env => env.WriteAllText(path, contents));
}