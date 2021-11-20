using static FunctionalSharp.F;

namespace FunctionalSharp.Wrappers; 

public static class Long {
    public static Maybe<long> Parse(string s) {
        if (long.TryParse(s, out long res))
            return res;

        return Nothing;
    }
}