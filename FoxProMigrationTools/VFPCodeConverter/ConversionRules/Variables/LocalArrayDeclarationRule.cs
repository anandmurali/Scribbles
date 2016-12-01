using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VFPCodeConverter.Common;
using VFPCodeConverter.ConversionMethods.Common;

namespace VFPCodeConverter.ConversionRules.Variables
{
    public class LocalArrayDeclarationRule : ConversionRuleBase, IConversionRule
    {
        #region Constructor

        public LocalArrayDeclarationRule()
        {
            RuleName = "LocalArrayDeclarationRule";
            RuleType = "LocalVariableDeclaration";
        }
        #endregion

        public bool IsRuleApplicable(string sourceCode, ConversionParameters conversionParameters)
        {
            if (conversionParameters.CopyComments == false)
                return false;

            // Get first Line
            var match = Regex.Match(sourceCode, RuleApplicablePattern);
            return match.Success;
        }

        public string ApplyConversionRule(string sourceCode, ConversionParameters conversionParameters)
        {
            Logger.AddApplyConversionRuleEntryLog(RuleName, RuleType, RuleTypePriority);

            var match = Regex.Match(sourceCode, RuleApplicablePattern);
            if (match.Success)
            {
                Logger.AddSourceCode(match.Value);

                string leadingSpace = match.Groups["leadingSpace"].ToString();

                StringBuilder convertedCodeBuilder = new StringBuilder();
                Dictionary<string, string> groupBuilder = new Dictionary<string, string>();
                foreach (Match subMatch in Regex.Matches(match.Value, SubPattern))
                {
                    string variableName = subMatch.Groups["variableName"].Value;


                    Utility.ConvertVariables(conversionParameters, groupBuilder, convertedCodeBuilder, variableName, leadingSpace, true);
                }

                if (conversionParameters.IsLocalVariableGroupingRequired)
                {
                    foreach (KeyValuePair<string, string> keyValuePair in groupBuilder)
                    {
                        convertedCodeBuilder.AppendLine(leadingSpace + keyValuePair.Key + " " + keyValuePair.Value + ";");
                    }
                }

                string convertedCode = convertedCodeBuilder.ToString();
                conversionParameters.AddConvertedCode(convertedCode);

                Logger.AddConvertedCode(convertedCode);

                sourceCode = sourceCode.Remove(match.Index, match.Length);

                if (sourceCode.Length > 0 && sourceCode.Substring(0, 1) == "\n")
                    sourceCode = sourceCode.Remove(0, 1);
                return sourceCode;
            }

            Logger.AddLog("MatchFailed");
            return sourceCode;
        }

    }
}
