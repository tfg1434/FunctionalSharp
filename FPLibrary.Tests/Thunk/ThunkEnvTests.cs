using System;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using FPLibrary;
using System.Collections.Generic;
using System.Linq;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.Thunk;

public class ThunkEnvTests {
    public static Thunk<IEnumerable<int>, int> LengthThunk()
        => Thunk<IEnumerable<int>, int>.Of(xs => xs.Count());
    
    [Property(Arbitrary = new[] { typeof(ArbitraryIEnumerable) })]
    public void Value_Int_UsesEnv(IEnumerable<int> env) {
        var thunk = LengthThunk();

        Assert.Equal(env.Count(), thunk.Value(env));
    }

    [Property(Arbitrary = new[] { typeof(ArbitraryIEnumerable) })]
    public void Value_Int_Memoizes(IEnumerable<int> env) {
        var thunk = LengthThunk();
        thunk.Value(env);

        env = env.Concat(new[] { 1, 2, 3 });
        Assert.NotEqual(env.Count(), thunk.Value(env));
    }

    [Property(Arbitrary = new[] { typeof(ArbitraryIEnumerable) })]
    public void ReValue_Int_NoMemoize(IEnumerable<int> env) {
        var thunk = LengthThunk();
        thunk.Value(env);
        
        env = env.Concat(new[] { 1, 2, 3 });
        Assert.Equal(env.Count(), thunk.ReValue(env));
    }
}