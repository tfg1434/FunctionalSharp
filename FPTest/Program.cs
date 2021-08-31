using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using static FPLibrary.F;
using FPLibrary;
using Unit = System.ValueTuple;
using static System.Diagnostics.Debug;

#nullable enable

Try<Uri> CreateUri(string uri) => Try(() => new Uri(uri));

Try<Uri> uri = CreateUri("http://github.com");

var b = uri.Run();

int a = 5;