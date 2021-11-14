using System;
using System.Text;
using FPLibrary;
using Unit = System.ValueTuple;

namespace FPLibrary.Wrappers.IO;

public interface IHasFile<E> where E : struct {
    IO<E, IFileIO> FileIO { get; }
}

public interface IFileIO {
    Unit AppendAllLines(string path, IEnumerable<string> lines);

    // Unit AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding);
    
    Unit AppendAllText(string path, string contents);
    
    // Unit AppendAllText(string path, string? contents, Encoding encoding);
    
    Unit Copy(string from, string to, bool overwrite = false);

    FileStream Create(string path);

    //FileStream Create(string path, int bufferSize);
    
    //FileStream Create(string path, int bufferSize, FileOptions options);

    StreamWriter CreateText(string path);

    //Unit Decrypt(string path);
    
    Unit Delete(string path);
    
    //Unit Encrypt(string path);
    
    bool Exists(string? path);
    
    //Get...
    
    //Move

    FileStream Open(string path, FileMode mode);

    // FileStream Open(string path, FileMode mode, FileAccess access);

    // FileStream Open(string path, FileMode mode, FileAccess access, FileShare share);
    
    FileStream OpenRead(string path);

    StreamReader OpenText(string path);
    
    FileStream OpenWrite(string path);
    
    byte[] ReadAllBytes(string path);

    string[] ReadAllLines(string path);
    
    // string[] ReadAllLines(string path, Encoding encoding);

    string ReadAllText(string path);

    // string ReadAllText(string path, Encoding encoding);

    IEnumerable<string> ReadLines(string path);
    
    // IEnumerable<string> ReadLines(string path, Encoding encoding);
    
    //Replace...
    
    //Set...
    
    Unit WriteAllBytes(string path, byte[] bytes);
    
    //why does this exist ( ´･･)ﾉ(._.`)
    //Unit WriteAllLines(string path, string[] lines); 
    
    Unit WriteAllLines(string path, IEnumerable<string> lines);
    
    // Unit WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding);

    Unit WriteAllText(string path, string? contents);
}