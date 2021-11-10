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
}

static class ArbitrarySuccThunk {
    public static Arbitrary<Thunk<T>> Thunk<T>() {
        Gen<Thunk<T>> gen = from value in Arb.Generate<T>()
                            select FPLibrary.Thunk<T>.OfSucc(value);

        return gen.ToArbitrary();
    }
}

static class ArbitraryFailThunk {
    public static Arbitrary<Thunk<T>> Thunk<T>() {
        Gen<Thunk<T>> gen = from ex in Arb.Generate<Exception>()
                            select FPLibrary.Thunk<T>.OfFail(new Error(ex));

        return gen.ToArbitrary();
    }
}

static class ArbitraryCancelledThunk {
    public static Arbitrary<Thunk<T>> Thunk<T>() {
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
    [Property(Arbitrary = new[] { typeof(ArbitraryThunk) })]
    public void IdentityHolds(Thunk<int> thunk) {
        Thunk<int> expected = thunk;
        Thunk<int> actual = thunk.Map(x => x);

        Assert.Equal(expected.Value(), actual.Value());
    }
    
    //fmap (f . g) == fmap f . fmap g
    [Property(Arbitrary = new[] { typeof(ArbitraryThunk) })]
    public void CompositionHolds(Thunk<int> thunk) {
        Thunk<int> expected = thunk.Map(Times2).Map(Plus5));
        Thunk<int> actual = thunk.Map(x => Plus5(Times2(x)));

        Assert.Equal(expected.Value(), actual.Value());
    }
}