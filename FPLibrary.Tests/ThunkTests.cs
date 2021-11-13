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
        Gen<Thunk<T>> gen = (from cancelled in Arb.Generate<bool>()
                             from isFail in Arb.Generate<bool>()
                             from value in Arb.Generate<T>()
                             select (cancelled, isFail) switch {
                                 (true, _) => FPLibrary.Thunk<T>.OfCancelled(),
                                 (_, false) => FPLibrary.Thunk<T>.OfFail(new Error()),
                                 _ => FPLibrary.Thunk<T>.OfSucc(value),
                             })!;

        return gen.ToArbitrary();
    }
}

static class ArbitrarySuccThunk {
    public static Arbitrary<Thunk<T>> Thunk<T>() {
        Gen<Thunk<T>> gen = (from value in Arb.Generate<T>()
                             select FPLibrary.Thunk<T>.OfSucc(value))!;

        return gen.ToArbitrary();
    }
}

static class ArbitraryFailThunk {
    public static Arbitrary<Thunk<T>> Thunk<T>() {
        Gen<Thunk<T>> gen = (from _ in Arb.Generate<bool>()
                             select FPLibrary.Thunk<T>.OfFail(new Error()))!;

        return gen.ToArbitrary();
    }
}

static class ArbitraryCancelledThunk {
    public static Arbitrary<Thunk<T>> Thunk<T>() {
        Gen<Thunk<T>> gen = (from _ in Arb.Generate<bool>()
                             select FPLibrary.Thunk<T>.OfCancelled())!;

        return gen.ToArbitrary();
    }
}

public class ThunkTests {
    [Property]
    public void Value_Succ_Int(int value) {
        var thunk = Thunk<int>.Of(() => value);
        var expected = new Result<int>(value);
        
        Assert.Equal(expected, thunk.Value());
        Assert.Equal(expected, thunk.ReValue());
        Assert.Equal(expected, thunk.Value());
    }

    [Fact]
    public void Value_Fail_Error() {
        var thunk = Thunk<int>.Of(() => new(new Error()));
        var expected = new Result<int>(new Error());
        
        Assert.Equal(expected, thunk.Value());

        thunk = Thunk<int>.OfFail(new Error());
        expected = new(new Error());
        Assert.Equal(expected, thunk.Value());
    }

    [Property(Arbitrary = new[] { typeof(ArbitrarySuccThunk) })]
    public void BiMap_Identity_Holds(Thunk<int> expected) {
        Thunk<int> actual = expected.BiMap(x => x, x => x);
        
        Assert.Equal(expected.Value(), actual.Value());
    }

    [Property(Arbitrary = new[] { typeof(ArbitraryThunk) })]
    public void BiMap_Composition_Holds(Thunk<int> thunk) {
        Func<int, int> f = Times2;
        Func<int, int> g = Plus5;
        Func<Error, Error> h = e => e;
        Func<Error, Error> i = e => e;

        Thunk<int> expected = thunk.BiMap(x => f(g(x)), x => h(i(x)));
        Thunk<int> actual = thunk.BiMap(g, i).BiMap(f, h);

        Assert.Equal(expected.Value(), actual.Value());
    }

    //map ident == ident
    [Property(Arbitrary = new[] { typeof(ArbitraryThunk) })]
    public void Map_Identity_Holds(Thunk<int> thunk) {
        Thunk<int> expected = thunk;
        Thunk<int> actual = thunk.Map(x => x);

        Assert.Equal(expected.Value(), actual.Value());
    }
    
    //fmap (f . g) == fmap f . fmap g
    [Property(Arbitrary = new[] { typeof(ArbitraryThunk) })]
    public void Map_Composition_Holds(Thunk<int> thunk) {
        Thunk<int> expected = thunk.Map(Times2).Map(Plus5);
        Thunk<int> actual = thunk.Map(x => Plus5(Times2(x)));

        Assert.Equal(expected.Value(), actual.Value());
    }
}