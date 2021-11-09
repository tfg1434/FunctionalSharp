using System;
using FsCheck.Xunit;
using Xunit;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests; 

public class ThunkTests {
    [Property]
    public void Value_Succ_Int(int value) {
        var thunk = Thunk<int>.OfSucc(() => value);
        var expected = new Result<int>(value);
        
        Assert.Equal(expected, thunk.Value());
        Assert.Equal(expected, thunk.ReValue());
        Assert.Equal(expected, thunk.Value());
    }

    [Property]
    public void Value_Fail_Error() {
        var thunk = Thunk<int>.OfSucc(() => new Error("e"));
        var expected = new Result<int>(new Error("e"));
        
        Assert.Equal(expected, thunk.Value());

        thunk = Thunk<int>.OfFail(new Error());
        expected = new(new Error());
        Assert.Equal(expected, thunk.Value());
    }
}