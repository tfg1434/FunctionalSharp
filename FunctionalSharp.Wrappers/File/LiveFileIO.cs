namespace FunctionalSharp.Wrappers.File;

public readonly struct LiveFileIO : IFileIO {
    public static readonly IFileIO Default = new LiveFileIO();

    public Unit AppendAllLines(string path, IEnumerable<string> lines) { 
        System.IO.File.AppendAllLines(path, lines);

        return Unit();
    }
    
    public Unit AppendAllText(string path, string contents) {
        System.IO.File.AppendAllText(path, contents);

        return Unit();
    }
    
    public Unit Copy(string from, string to, bool overwrite = false) {
        System.IO.File.Copy(from, to, overwrite);

        return Unit();
    }
    
    public FileStream Create(string path)
        => System.IO.File.Create(path);

    public StreamWriter CreateText(string path) 
        => System.IO.File.CreateText(path);

    public Unit Delete(string path) {
        System.IO.File.Delete(path);
        
        return Unit();
    }
    
    public bool Exists(string? path) 
        => System.IO.File.Exists(path);
    
    public FileStream Open(string path, FileMode mode)
        => System.IO.File.Open(path, mode);
    
    public FileStream OpenRead(string path)
        => System.IO.File.OpenRead(path);
    
    public StreamReader OpenText(string path)
        => System.IO.File.OpenText(path);
    
    public FileStream OpenWrite(string path)
        => System.IO.File.OpenWrite(path);
    
    public byte[] ReadAllBytes(string path)
        => System.IO.File.ReadAllBytes(path);
    
    public string[] ReadAllLines(string path)
        => System.IO.File.ReadAllLines(path);

    public string ReadAllText(string path) 
        => System.IO.File.ReadAllText(path);
    
    public IEnumerable<string> ReadLines(string path)
        => System.IO.File.ReadLines(path);
    
    public Unit WriteAllBytes(string path, byte[] bytes) {
        System.IO.File.WriteAllBytes(path, bytes);

        return Unit();
    }
    
    public Unit WriteAllLines(string path, IEnumerable<string> lines) {
        System.IO.File.WriteAllLines(path, lines);

        return Unit();
    }

    public Unit WriteAllText(string path, string? contents) {
        System.IO.File.WriteAllText(path, contents);

        return Unit();
    }
}