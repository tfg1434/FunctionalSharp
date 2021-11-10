using System;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using FPLibrary;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests;

static class ArbitraryThunk {
    public static Arbitrary<Thunk<T>> Thunk<T>() {
        Gen<Thunk<T>> gen = from cancelled in Arb.Generate<bool>()
                            from isFail in Arb.Generate<bool>()
                            from value in Arb.Generate<T>()
                            select (cancelled, isFail) switch {
                                (true, _) => FPLibrary.Thunk<T>.OfCancelled(),
                                (_, false) => FPLibrary.Thunk<T>.OfFail(new Error()),
                                _ => FPLibrary.Thunk<T>.OfSucc(value),
                            };

        return gen.ToArbitrary();
    }
    
    public static Arbitrary<Thunk<T>> SuccThunk<T>() {
        Gen<Thunk<T>> gen = from value in Arb.Generate<T>()
                            select FPLibrary.Thunk<T>.OfSucc(value);

        return gen.ToArbitrary();
    }
    
    public static Arbitrary<Thunk<T>> FailThunk<T>() {
        Gen<Thunk<T>> gen = from ex in Arb.Generate<Exception>()
                            select FPLibrary.Thunk<T>.OfFail(new Error(ex));

        return gen.ToArbitrary();
    }
    
    public static Arbitrary<Thunk<T>> CancelledThunk<T>() {
        Gen<Thunk<T>> gen = from _ in Arb.Generate<bool>()
                            select FPLibrary.Thunk<T>.OfCancelled();

        return gen.ToArbitrary();
    }
}

public class ThunkTests {
    [Property]
    public void Value_Succ_Int(int value) {
        var thunk = Thunk<int>.OfSucc(() => value);
        var expected = new Result<int>(value);
        
        Assert.Equal(expected, thunk.Value());
        Assert.Equal(expected, thunk.ReValue());
        Assert.Equal(expected, thunk.Value());
    }

    [Fact]
    public void Value_Fail_Error() {
        var thunk = Thunk<int>.OfSucc(() => new(new Error()));
        var expected = new Result<int>(new Error());
        
        Assert.Equal(expected, thunk.Value());

        thunk = Thunk<int>.OfFail(new Error());
        expected = new(new Error());
        Assert.Equal(expected, thunk.Value());
    }
    
    //map ident == ident
    [Fact]
    public void IdentityHolds()
}