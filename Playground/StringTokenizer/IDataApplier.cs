using System.Collections.Generic;

namespace JDM.Playground.StringTokenizer
{
    public interface IDataApplier
    {
        IEnumerable<string> FillTokensWithData(IEnumerable<string> tokenList, IDictionary<RecognizedTokenValue, string> tokenData);
    }
}