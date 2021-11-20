#pragma warning disable CS8619

namespace FunctionalSharp.Tests.Maybe {
    static class ArbitraryMaybe {
        public static Arbitrary<Maybe<T>> Maybe<T>() {
            Gen<Maybe<T>> gen = from isJust in Arb.Generate<bool>() 
                                from val in Arb.Generate<T>() 
                                select isJust && val is not null ? Just(val) : Nothing;

            return gen.ToArbitrary();
        }
    }

    public class MaybeTests {
        [Fact]
        public void Match_Int_Just() {
            GetValue(true)
                .Match(
                    Fail,
                    _ => Succeed());
        }

        [Fact]
        public void Match_String_Nothing() {
            GetValue(false)
                .Match(
                    Succeed,
                    _ => Fail());
        }

        [Property]
        public void Map_Int_Just(int x) {
            Maybe<int> m = Just(x);
            m.Map(Times2);

            Assert.True(m.IsJust);
        }

        [Fact]
        public void Map_Int_Nothing() {
            Maybe<int> m = Nothing;
            m.Map(Times2);

            Assert.True(m.IsNothing);
        }


        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void LinqQuery_SingleClause(Maybe<int> m) {
            Maybe<int> expected = m.Map(Times2);
            Maybe<int> actual = from x in m 
                                select Times2(x);
            
            Assert.Equal(expected, actual);
        }

        [Property(Arbitrary = new[] { typeof(ArbitraryMaybe) })]
        public void LinqQuery_TwoClause(Maybe<int> mA, Maybe<int> mB) {
            Maybe<int> expected = mA.Bind(a => mB.Map(b => a + b));
            Maybe<int> actual = from a in mA
                                from b in mB
                                select a + b;

            Assert.Equal(expected, actual);
        }

        [Property(Arbitrary = new[] {typeof(ArbitraryMaybe)})]
        public void LinqQuery_ThreeClause(Maybe<int> mA, Maybe<int> mB, Maybe<int> mC) {
            Maybe<int> expected = 
                mA.Bind(a => mB.Bind(
                            b => mC.Map(
                                c => a + b + c)));
            Maybe<int> actual = from a in mA
                                from b in mB
                                from c in mC
                                select a + b + c;
            
            Assert.Equal(expected, actual);
        }

        private static Maybe<int> GetValue(bool select)
            => select ? Just(0) : Nothing;
    }
}
