using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VFPCodeConverter.Common;
using VFPCodeConverter.ConversionMethods.Common;

namespace VFPCodeConverter
{
    public class GenericConversionRule : ConversionRuleBase, IConversionRule
    {
        #region Properties

        public GenericConversionRuleInfo ConversionRule { get; set; }
        #endregion

        #region Constructors

        public GenericConversionRule(GenericConversionRuleInfo conversionRule)
        {
            ConversionRule = conversionRule;

            RuleName = ConversionRule.RuleName;
            RuleType = ConversionRule.RuleType;
            RuleTypePriority = ConversionRule.RuleTypePriority;
            GlobalPriority = ConversionRule.GlobalPriority;
            RuleApplicablePattern = ConversionRule.RuleApplicablePattern;
        }
        #endregion


        #region IConversionRule Implementation
        public bool IsRuleApplicable(string sourceCode, ConversionParameters conversionParameters)
        {
            // Get first Line
            var regex = new Regex(RuleApplicablePattern);
            return regex.IsMatch(sourceCode);
        }

        public string ApplyConversionRule(string sourceCode, ConversionParameters conversionParamters)
        {
            Logger.AddLog(String.Format("ApplyingRule Name: {0}-Type: {1}-Priority: {2}", RuleName, RuleType, RuleTypePriority));

            var match = Regex.Match(sourceCode, RuleApplicablePattern);
            if (match.Success)
            {
                Logger.AddLog("SourceCode:" + match.Value);

                var regex = new Regex(RuleApplicablePattern);
                var convertedCode = regex.Replace(match.Value, ConversionRule.ReplacePattern);
                conversionParamters.AddConvertedCode(convertedCode);

                Logger.AddLog("ConvertedCode:" + convertedCode);

                return sourceCode.Remove(match.Index, match.Length);
            }

            Logger.AddLog("MatchFailed");
            return sourceCode;
        }
        #endregion

    }
}
