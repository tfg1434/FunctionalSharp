using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FPLibrary;
using static FPLibrary.F;

namespace FPLibrary {
    public static partial class F {
        public static IEnumerable<int> Range(string range)
            => FPLibrary.Range.Of(range).IfNothing(
                () => throw new ArgumentException("Invalid range.", nameof(range)));
    }
    
    public class Range : IEnumerable<int> {
        private readonly int _from;
        private readonly int? _to;
        private readonly int _step;
        private readonly bool _isAscending;

        private bool isInfinite => _to is null;

        private Range(int from, int? to, int step = 1) {
            _from = from;
            _to = to;
            _step = step;
            _isAscending = step > 0;
        }

        public static Maybe<Range> Of(string range) {
            const string pattern_from_second_to = @"^(?<from>-?[0-9]+)(?:,(?<second>-?[0-9]+))\.\.(?<to>-?[0-9]+)$";
            const string pattern_from_second    = @"^(?<from>-?[0-9]+)(?:,(?<second>-?[0-9]+))\.\.$";
            const string pattern_from_to        = @"^(?<from>-?[0-9]+)\.\.(?<to>-?[0-9]+)$";
            const string pattern_from           = @"^(?<from>-?[0-9]+)\.\.$";

            var match = Regex.Match(range, pattern_from_second_to);

            if (match.Success) {
                var from = int.Parse(match.Groups["from"].Value);
                var second = int.Parse(match.Groups["second"].Value);
                var to = int.Parse(match.Groups["to"].Value);
                
                return new Range(from, to, second - from);
            }

            match = Regex.Match(range, pattern_from_second);

            if (match.Success) {
                var from = int.Parse(match.Groups["from"].Value);
                var second = int.Parse(match.Groups["second"].Value);

                return new Range(from, null, second - from);
            }

            match = Regex.Match(range, pattern_from_to);

            if (match.Success) {
                var from = int.Parse(match.Groups["from"].Value);
                var to = int.Parse(match.Groups["to"].Value);

                return new Range(from, to);
            }

            match = Regex.Match(range, pattern_from);

            if (match.Success) {
                var from = int.Parse(match.Groups["from"].Value);

                return new Range(from, null);
            }

            return Nothing;
        }

        public IEnumerable<int> AsEnumerable() {
            if (_isAscending)
                for (int x = _from; isInfinite || x <= _to; x += _step)
                    yield return x;
            else
                for (int x = _from; isInfinite || x >= _to; x += _step)
                    yield return x;
        }
        
        public IEnumerator<int> GetEnumerator() => AsEnumerable().GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
    }
}