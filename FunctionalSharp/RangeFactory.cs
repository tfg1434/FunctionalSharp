// using System;
// using System.Text.RegularExpressions;
//
// namespace FunctionalSharp;
// using static FunctionalSharp.F;
//
// public partial class Range<T> {
//     private static Maybe<Range<U>> Of<U>(string typePattern, Func<string, U> parse, U empty, Func<U, U, U> add,
//         Func<U, U, U> subtract, Func<U, U, int> compare, U one, string range) {
//
//         Range<U> Ctor(U from, U? to, U step) => new(from, to, step, empty, compare, add);
//         
//         string pattern = $@"^(?<from>-?{typePattern})(?:,(?<second>-?{typePattern}))?\.\.(?<to>-?{typePattern})?$";
//         var match = Regex.Match(range, pattern);
//
//         Group from = match.Groups["from"];
//
//         if (!from.Success)
//             return Nothing;
//
//         U fromU = parse(from.Value);
//         Group second = match.Groups["second"];
//         Group to = match.Groups["to"];
//
//         return (second.Success, to.Success) switch {
//             //from, second, to
//             (true, true) => Ctor(fromU, parse(to.Value), subtract(parse(second.Value), fromU)),
//             //from, second
//             //TODO: why cant i use null instead of default
//             (true, false) => Ctor(fromU, default, subtract(parse(second.Value), fromU)),
//             //from, to
//             (false, true) => Ctor(fromU, parse(to.Value), one),
//             //from
//             (false, false) => Ctor(fromU, default, one),
//         };
//     }
// }