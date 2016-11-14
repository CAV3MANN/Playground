using JDM.Playground.StringTokenizer;
using System.Collections.Generic;

namespace AssortedTests.StringTokenizer
{
    public sealed class FooExtractor : DataExtractor<Foo>
    {
        public override IDictionary<RecognizedTokenValue, string> ExtractFrom(Foo item)
        {
            var dataDictionary = new Dictionary<RecognizedTokenValue, string>();
            dataDictionary.Add(RecognizedTokenValue.F, item.First);
            dataDictionary.Add(RecognizedTokenValue.M, item.Middle);
            dataDictionary.Add(RecognizedTokenValue.L, item.Last);
            dataDictionary.Add(RecognizedTokenValue.SUFFIX, item.Suffix);

            return dataDictionary;
        }
    }
}
