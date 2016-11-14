using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JDM.Playground.StringTokenizer
{
    public sealed class Tokenizer : ITokenizer
    {
        public IEnumerable<string> Tokenize(string toTokenize)
        {
            var tokens = new List<string>();
            var anythingInsideBrackets = @"((?<=\{).+?(?=\}))";
            var anyComma = @"(,)";
            var anySpace = @"( )";
            var anyNewLine = @"(\n)";
            var tokenRegex = new Regex(string.Join("|", anythingInsideBrackets, anyComma, anyNewLine, anySpace));
            var matches = tokenRegex.Matches(toTokenize).Cast<Match>().Select(match => match.Value).ToList();
            return matches;
        }
    }
}
