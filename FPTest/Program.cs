using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using static FPLibrary.F;
using FPLibrary;
using Unit = System.ValueTuple;
using static System.Diagnostics.Debug;

#nullable enable

Func<int, int> times2 = x => x * 2;
Func<int, int> plus5 = x => x + 5;

Maybe<int> m = Just(5);

Maybe<int> expected = m.Map(times2).Map(plus5);
Maybe<int> actual = m.Map(x => times2(plus5(x)));

