namespace FunctionalSharp.Wrappers; 

public readonly struct LiveConsoleIO : IConsoleIO {
    public static readonly IConsoleIO Default = new LiveConsoleIO();

    public ConsoleColor BgColour => Console.BackgroundColor;
    
    public ConsoleColor FgColour => Console.ForegroundColor;
    
    public Unit Clear() {
        Console.Clear();

        return Unit();
    }

    public Maybe<int> Read() {
        int n = Console.Read();
        
        return n == -1 ? Nothing : Just(n);
    }

    public ConsoleKeyInfo ReadKey() 
        => Console.ReadKey();

    public Maybe<string> ReadLine()
        => Jull(Console.ReadLine());

    public Unit ResetColour() {
        Console.ResetColor();

        return Unit();
    }
    
    public Unit SetBgColour(ConsoleColor colour) {
        Console.BackgroundColor = colour;

        return Unit();
    }
    
    public Unit SetFgColour(ConsoleColor colour) {
        Console.ForegroundColor = colour;

        return Unit();
    }
    
    public Unit Write(string value) {
        Console.Write(value);

        return Unit();
    }

    public Unit Write(char value) {
        Console.Write(value);

        return Unit();
    }

    public Unit WriteLine(string line) {
        Console.WriteLine(line);

        return Unit();
    }
    
    public Unit WriteLine() {
        Console.WriteLine();

        return Unit();
    }
}