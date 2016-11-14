using System;
using System.Collections.Generic;
using System.Linq;

namespace JDM.Playground.StringTokenizer
{
    public sealed class DataApplier : IDataApplier
    {
        private string[] recognizedFormatTokens = { ",", "\n" };

        public IEnumerable<string> FillTokensWithData(IEnumerable<string> tokenList, IDictionary<RecognizedTokenValue, string> tokenData)
        {
            var filledTokens = new List<string>();
            foreach (var token in tokenList)
            {
                RecognizedTokenValue recognizedToken;
                var matchedDataItem = "";
                if (Enum.TryParse(token.ToUpper(), out recognizedToken))
                {
                    if (tokenData.TryGetValue(recognizedToken, out matchedDataItem))
                    {
                        filledTokens.Add(matchedDataItem);
                    }
                    else
                    {
                        filledTokens.Add(" ");
                    }
                }
                else if (recognizedFormatTokens.Contains(token))
                {
                    filledTokens.Add(token);
                }
            }

            return filledTokens;
        }
    }
}
