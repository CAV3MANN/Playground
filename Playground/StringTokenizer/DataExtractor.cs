using System.Collections.Generic;

namespace JDM.Playground.StringTokenizer
{
    public abstract class DataExtractor<TDataSourceType>
    {
        public abstract IDictionary<RecognizedTokenValue, string> ExtractFrom(TDataSourceType item);
    }
}