using System.Collections.Generic;
using System.Linq;

namespace JDM.Playground.StringTokenizer
{
    public sealed class RuleApplier : IRuleApplier
    {
        private List<string> tokenList;
        private string[] recognizedFormatTokens = { ",", "\n" };

        public IEnumerable<string> ApplyRulesToTokens(IEnumerable<string> tokens)
        {
            this.tokenList = tokens.ToList();
            RemoveAllSpaces();
            RemoveUnneededFormatting();
            AddSpacesAfterCommasAndBetweenStrings();
            return tokenList;
        }

        private void RemoveAllSpaces()
        {
            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i] == " ")
                {
                    tokenList.RemoveAt(i);
                    i--;
                }
            }
        }

        private void AddSpacesAfterCommasAndBetweenStrings()
        {
            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i] == ",")
                {
                    tokenList.Insert(i + 1, " ");
                    i++;
                }
                else if ((i + 1) < tokenList.Count && (!recognizedFormatTokens.Contains(tokenList[i]) && !recognizedFormatTokens.Contains(tokenList[i + 1])))
                {
                    tokenList.Insert(i + 1, " ");
                    i++;
                }
            }
        }

        private void RemoveUnneededFormatting()
        {
            for (int i = 0; i < tokenList.Count; i++)
            {
                var currentIsComma = tokenList[i] == ",";
                var currentIsNewLine = tokenList[i] == "\n";
                var IsNotLast = tokenList.Count > (i + 1);
                var nextIsComma = IsNotLast ? tokenList[i + 1] == "," : false;
                var nextIsNewLine = IsNotLast ? tokenList[i + 1] == "\n" : false;
                var IsNotFirst = i > 0;
                var previousIsNewLine = IsNotFirst ? tokenList[i - 1] == "\n" : false;

                while (!IsNotFirst && (currentIsComma || currentIsNewLine))
                {
                    tokenList.RemoveAt(i);

                    currentIsComma = tokenList[i] == ",";
                    currentIsNewLine = tokenList[i] == "\n";
                    IsNotLast = tokenList.Count > (i + 1);
                    nextIsComma = IsNotLast ? tokenList[i + 1] == "," : false;
                    nextIsNewLine = IsNotLast ? tokenList[i + 1] == "\n" : false;
                    previousIsNewLine = IsNotFirst ? tokenList[i - 1] == "\n" : false;
                }

                if (currentIsComma && (nextIsComma || nextIsNewLine))
                {
                    tokenList.RemoveAt(i);
                    i--;
                }
                else if (currentIsComma && previousIsNewLine)
                {
                    tokenList.RemoveAt(i);
                    i--;
                }
                else if (!IsNotLast && (currentIsComma || currentIsNewLine))
                {
                    tokenList.RemoveAt(i);
                    i--;
                }
                else if (currentIsNewLine && nextIsNewLine)
                {
                    tokenList.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
