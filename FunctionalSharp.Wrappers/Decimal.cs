using static FunctionalSharp.F;

namespace FunctionalSharp.Wrappers; 

public static class Decimal {
    public static Maybe<decimal> Parse(string s) {
        if (decimal.TryParse(s, out decimal res))
            return res;

        return Nothing;
    }
}