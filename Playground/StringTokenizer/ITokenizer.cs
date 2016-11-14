using System.Collections.Generic;

namespace JDM.Playground.StringTokenizer
{
    public interface ITokenizer
    {
        IEnumerable<string> Tokenize(string toTokenize);
    }
}