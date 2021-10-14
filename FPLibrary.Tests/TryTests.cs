using System;
using Xunit;
using static FPLibrary.F;
using static FPLibrary.Tests.Utils;

namespace FPLibrary.Tests {
    public class TryTests {
        private Try<Uri> CreateUri(string uri) => Try(() => new Uri(uri));

        [Fact]
        public void Try_Uri_NotNullUri() {
            Try<Uri> uri = CreateUri("http://github.com");

            uri.Run().Match(
                _ => Fail(),
                Assert.NotNull);
        }

        [Fact]
        public void Try_Uri_NotNullEx() {
            Try<Uri> uri = CreateUri("");

            uri.Run().Match(
                Assert.NotNull,
                _ => Fail());
        }
    }
}
