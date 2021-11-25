namespace FunctionalSharp.Wrappers;

public readonly struct LiveFileIO : IFileIO {
    public static readonly IFileIO Default = new LiveFileIO();

    public Unit AppendAllLines(string path, IEnumerable<string> lines) { 
        File.AppendAllLines(path, lines);

        return Unit();
    }
    
    public Unit AppendAllText(string path, string contents) {
        File.AppendAllText(path, contents);

        return Unit();
    }
    
    public Unit Copy(string from, string to, bool overwrite = false) {
        File.Copy(from, to, overwrite);

        return Unit();
    }
    
    public FileStream Create(string path)
        => File.Create(path);

    public StreamWriter CreateText(string path) 
        => File.CreateText(path);

    public Unit Delete(string path) {
        File.Delete(path);
        
        return Unit();
    }
    
    public bool Exists(string? path) 
        => File.Exists(path);
    
    public FileStream Open(string path, FileMode mode)
        => File.Open(path, mode);
    
    public FileStream OpenRead(string path)
        => File.OpenRead(path);
    
    public StreamReader OpenText(string path)
        => File.OpenText(path);
    
    public FileStream OpenWrite(string path)
        => File.OpenWrite(path);
    
    public byte[] ReadAllBytes(string path)
        => File.ReadAllBytes(path);
    
    public string[] ReadAllLines(string path)
        => File.ReadAllLines(path);

    public string ReadAllText(string path) 
        => File.ReadAllText(path);
    
    public IEnumerable<string> ReadLines(string path)
        => File.ReadLines(path);
    
    public Unit WriteAllBytes(string path, byte[] bytes) {
        File.WriteAllBytes(path, bytes);

        return Unit();
    }
    
    public Unit WriteAllLines(string path, IEnumerable<string> lines) {
        File.WriteAllLines(path, lines);

        return Unit();
    }

    public Unit WriteAllText(string path, string? contents) {
        File.WriteAllText(path, contents);

        return Unit();
    }
}