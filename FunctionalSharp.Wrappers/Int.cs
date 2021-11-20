using static FunctionalSharp.F;

namespace FunctionalSharp.Wrappers; 

public static class Int {
    public static Maybe<int> Parse(string s) {
        if (int.TryParse(s, out int res))
            return res;

        return Nothing;
    }
}