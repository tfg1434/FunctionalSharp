using System;
using Unit = System.ValueTuple;
using static FPLibrary.F;

namespace FPLibrary; 

public static partial class F {

}

public readonly struct IO<E, T> where E : struct {
    private readonly Thunk<E, T> _thunk;
    
    internal IO(Thunk<E, T> thunk) => _thunk = thunk;

    public Result<T> Run(E env) => _thunk.Value(env);

    public Result<T> ReRun(E env) => _thunk.ReValue(env);

    public IO<E, T> Clone() => new(_thunk.Clone());
    
    
}
