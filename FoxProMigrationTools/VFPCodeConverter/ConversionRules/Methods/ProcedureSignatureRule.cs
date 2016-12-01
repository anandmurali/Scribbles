using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VFPCodeConverter.Common;
using VFPCodeConverter.ConversionMethods.Common;
using WPFLibrary.Extensions;

namespace VFPCodeConverter.ConversionRules.Methods
{
    public class ProcedureSignatureRule : ConversionRuleBase, IConversionRule
    {
        #region Constructor

        public ProcedureSignatureRule()
        {
            RuleName = "ProcedureSignatureRule";
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
                string methodName = match.Groups["methodName"].ToString();
                string methodParameters = match.Groups["methodParameters"].ConvertToString();
                string comments = match.Groups["comments"].ConvertToString();

                conversionParameters.AddMethodName(leadingSpace, methodName, comments);

                foreach (var variableNameWithSpace in methodParameters.Split(','))
                {
                    string variableName = variableNameWithSpace.Trim();
                    conversionParameters.AddMethodParameters(Utility.GetType(variableName), variableName);
                }

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
