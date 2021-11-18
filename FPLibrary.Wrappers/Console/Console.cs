using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary.Wrappers.Console;

public static class Console<E> where E : struct, IHasConsole<E> {
    public static IO<E, Unit> Clear()
        => default(E).ConsoleIO.Map(env => env.Clear());
    
    public static IO<E, int> Read()
        => default(E).ConsoleIO
            .Bind(env => env.Read()
                .Match(
                    () => EffFail<int>(new IOError("End of stream")),
                    EffSucc<int>));

    public static IO<E, ConsoleKeyInfo> ReadKey()
        => default(E).ConsoleIO.Map(env => env.ReadKey());
    
    
}