using System.Collections.Generic;

namespace JDM.Playground.StringTokenizer
{
    public interface IRuleApplier
    {
        IEnumerable<string> ApplyRulesToTokens(IEnumerable<string> tokens);
    }
}