using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VFPCodeConverter.Common;
using VFPCodeConverter.ConversionMethods.Common;

namespace VFPCodeConverter.ConversionRules.Comments
{
    public class StarCommentRule : ConversionRuleBase, IConversionRule
    {
        #region Constructor

        public StarCommentRule()
        {
            RuleName = "StarCommentRule";
            RuleType = "Comment";
        }
        #endregion

        public bool IsRuleApplicable(string sourceCode, ConversionParameters conversionParameters)
        {
            // Get first Line
            var match = Regex.Match(sourceCode, RuleApplicablePattern);
            return match.Success;
        }

        public string ApplyConversionRule(string sourceCode, ConversionParameters conversionParameters)
        {
            Logger.AddLog(String.Format("ApplyingRule Name:{0}-Type:{1}-Priority:{2}", RuleName, RuleType, RuleTypePriority));

            var match = Regex.Match(sourceCode, RuleApplicablePattern);
            if (match.Success)
            {
                Logger.AddLog("SourceCode:" + match.Value);

                string comment = match.Groups["comment"].ToString();

                var convertedCode = @"\\ " + comment.Trim();

                if (conversionParameters.CopyComments == false)
                    conversionParameters.AddConvertedCode(convertedCode);

                Logger.AddLog("ConvertedCode:" + convertedCode);

                return sourceCode.Remove(match.Index, match.Length);
            }

            Logger.AddLog("MatchFailed");
            return sourceCode;
        }
    }
}
