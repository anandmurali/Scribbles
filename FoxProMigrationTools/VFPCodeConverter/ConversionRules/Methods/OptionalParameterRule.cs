using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VFPCodeConverter.CodeBuilders;
using VFPCodeConverter.Common;
using VFPCodeConverter.ConversionMethods.Common;
using WPFLibrary.Extensions;

namespace VFPCodeConverter.ConversionRules.Methods
{
    public class OptionalParameterRule : ConversionRuleBase, IConversionRule
    {
        #region Constructor

        public OptionalParameterRule()
        {
            RuleName = "OptionalParameterRule";
            RuleType = "Method";
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
                int parameterCount = match.Groups["parameterCount"].ConvertToInt32();
                string parameterName = match.Groups["parameterName"].ToString();
                string defaultValue = match.Groups["defaultValue"].ConvertToString();

                conversionParameters.AddOptionalParameterValue(parameterName, defaultValue, parameterCount);

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
