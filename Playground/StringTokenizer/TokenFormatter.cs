using System.Collections.Generic;

namespace JDM.Playground.StringTokenizer
{
    public sealed class TokenFormatter<TDataSourceType> : ITokenFormatter<TDataSourceType>
    {
        private readonly DataExtractor<TDataSourceType> extractor;
        private readonly ITokenizer tokenizer;
        private readonly IDataApplier tokenFiller;
        private readonly IRuleApplier ruleApplier;

        public TokenFormatter(DataExtractor<TDataSourceType> extractor,
                              ITokenizer tokenizer,
                              IDataApplier tokenFiller,
                              IRuleApplier ruleApplier)
        {
            this.extractor = extractor;
            this.tokenizer = tokenizer;
            this.tokenFiller = tokenFiller;
            this.ruleApplier = ruleApplier;
        }

        public string Format(TDataSourceType dataToFormat, string formatString)
        {
            var tokens = this.tokenizer.Tokenize(formatString);
            var data = this.extractor.ExtractFrom(dataToFormat);
            var filledTokens = this.tokenFiller.FillTokensWithData(tokens, data);
            var ruledTokens = this.ruleApplier.ApplyRulesToTokens(filledTokens);

            return ConvertTokensToString(ruledTokens);
        }

        private string ConvertTokensToString(IEnumerable<string> tokens)
        {
            return string.Join("", tokens);
        }
    }
}
