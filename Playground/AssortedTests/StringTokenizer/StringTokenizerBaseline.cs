using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using JDM.Playground.StringTokenizer;

namespace AssortedTests.StringTokenizer
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public sealed class StringTokenizerBaseline
    {
        [TestMethod]
        public void GeneralTokenizerTest()
        {
            var formatString = "{P},{P}, {F},{L}, {P}\n{P}\n{P}\n{M} {Suffix} .... {asdfasdfasdf}";
            var foo = new Foo()
            {
                First = "Joshua",
                Last = "Miller",
                Middle = "Boop",
                Suffix = "Dr"
            };

            var tokenFormatter = new TokenFormatter<Foo>(new FooExtractor(), new Tokenizer(), new DataApplier(), new RuleApplier());
            var output = tokenFormatter.Format(foo, formatString);
        }
    }
}
