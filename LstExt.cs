using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLibrary {
    using static F;

    public static partial class F {
        public static ImmutableList<T> List<T>(params T[] items) 
            => items.ToImmutableList(); 
    }

    public static class LstExt {
        
    }
}
