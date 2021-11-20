using static FunctionalSharp.F;

namespace FunctionalSharp.Wrappers; 

public static class Double {
    public static Maybe<double> Parse(string s) {
        if (double.TryParse(s, out double res))
            return res;

        return Nothing;
    }
}