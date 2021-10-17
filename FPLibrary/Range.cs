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
            const string type_pattern = @"[0-9]+";
            const string pattern = $@"^(?<from>-?{type_pattern})(?:,(?<second>-?{type_pattern}))?\.\.(?<to>-?{type_pattern})?$";

            var match = Regex.Match(range, pattern);

            Group from = match.Groups["from"];
            
            if (!from.Success)
                return Nothing;
            
            int fromInt = int.Parse(from.Value);
            Group second = match.Groups["second"];
            Group to = match.Groups["to"];

            return Just<Range>((second.Success, to.Success) switch {
                //from, second, to
                (true, true) => new(fromInt, int.Parse(to.Value), int.Parse(second.Value) - fromInt),
                //from, second
                (true, false) => new(fromInt, null, int.Parse(second.Value) - fromInt),
                //from, to
                (false, true) => new(fromInt, int.Parse(to.Value)),
                //from
                (false, false) => new(fromInt, null),
            });
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