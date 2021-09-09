using System;
using Xunit;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Xunit;
using System.Collections.Generic;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests.IEnumerable {
    public class MonadLawTests {
        //m == m >>= Return
        [Property]
        public void RightIdentityHolds(IEnumerable<object> x) {
            
        }

        //Return t >== f == f t
        [Property]
        public void LeftIdentityHolds(NonNull<object> x) {
            
        }

        //(m >>= f) >>= g == m >>= (x => f(x) >>= g)
        [Property]
        public void AssociativeHolds(Maybe<int> m) {
            
        }
    }
}
