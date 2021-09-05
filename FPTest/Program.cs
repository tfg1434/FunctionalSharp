using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using static FPLibrary.F;
using FPLibrary;
using Unit = System.ValueTuple;
using static System.Diagnostics.Debug;

#nullable enable

Func<int, int, int> Add = (x, y) => x + y;

Maybe<int> expected = Just(3);
Maybe<int> actual = Just(Add)
    .Apply(Just(1))
    .Apply(Just(2));

int a = 5;

