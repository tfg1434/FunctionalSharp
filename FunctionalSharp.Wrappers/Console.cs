﻿namespace FunctionalSharp.Wrappers;

public static class Console<E> where E : struct, IHasConsole<E> {
    public static IO<E, ConsoleColor> BgColour
        => default(E).ConsoleIO.Map(env => env.BgColour);
    
    public static IO<E, ConsoleColor> FgColour
        => default(E).ConsoleIO.Map(env => env.FgColour);
    
    public static IO<E, Unit> Clear()
        => default(E).ConsoleIO.Map(env => env.Clear());
    
    public static IO<E, int> Read()
        => default(E).ConsoleIO
            .Bind<int>(env => env.Read()
                .Match(
                    () => EffFail<int>(new IOError("End of stream")),
                    EffSucc<int>));

    public static IO<E, ConsoleKeyInfo> ReadKey()
        => default(E).ConsoleIO.Map(env => env.ReadKey());

    public static IO<E, string> ReadLine()
        => default(E).ConsoleIO
            .Bind<string>(env => env.ReadLine()
                .Match(() => EffFail<string>(new IOError("no more characters available")),
                    EffSucc<string>));

    public static IO<E, Unit> ResetColour()
        => default(E).ConsoleIO.Map(env => env.ResetColour());
    
    public static IO<E, Unit> SetBgColour(ConsoleColor colour)
        => default(E).ConsoleIO.Map(env => env.SetBgColour(colour));

    public static IO<E, Unit> SetFgColour(ConsoleColor colour)
        => default(E).ConsoleIO.Map(env => env.SetFgColour(colour));
    
    public static IO<E, Unit> Write(string value)
        => default(E).ConsoleIO.Map(env => env.Write(value));
    
    public static IO<E, Unit> Write(char value)
        => default(E).ConsoleIO.Map(env => env.Write(value));

    public static IO<E, Unit> WriteLine(string line)
        => default(E).ConsoleIO.Map(env => env.WriteLine(line));
    
    public static IO<E, Unit> WriteLine()
        => default(E).ConsoleIO.Map(env => env.WriteLine());
    
    

}