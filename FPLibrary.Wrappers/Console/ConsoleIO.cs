using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary.Wrappers.Console; 

public readonly struct ConsoleIO : IConsoleIO {
    public static readonly IConsoleIO Default = new ConsoleIO();

    public 
    
    public Unit Clear() {
        System.Console.Clear();

        return Unit();
    }
    

}