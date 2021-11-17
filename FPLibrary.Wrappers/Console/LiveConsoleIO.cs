using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary.Wrappers.Console; 

public readonly struct ConsoleIO : IConsoleIO {
    public static readonly IConsoleIO Default = new ConsoleIO();

    public ConsoleColor BgColour => System.Console.BackgroundColor;
    
    public ConsoleColor FgColour => System.Console.ForegroundColor;
    
    public Unit Clear() {
        System.Console.Clear();

        return Unit();
    }

    public Maybe<int> Read() {
        int n = System.Console.Read();
        
        return n == -1 ? Nothing : Just(n);
    }

    public ConsoleKeyInfo ReadKey() 
        => System.Console.ReadKey();

    public Maybe<string> ReadLine()
        => Jull(System.Console.ReadLine());

    public Unit Write(string value) {
        System.Console.Write(value);

        return Unit();
    }
    
    public Unit WriteLine(string value) {
        System.Console.WriteLine(value);

        return Unit();
    }
    
    public Unit WriteLine() {
        System.Console.WriteLine();

        return Unit();
    }
}