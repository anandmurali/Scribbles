using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFPCodeConverter
{
    public interface IConversionRule
    {
        string RuleName { get; set; }

        string RuleType { get; set; }

        int RuleTypePriority { get; set; }

        int GlobalPriority { get; set; }

        bool IsRuleApplicable(string sourceCode, ConversionParameters conversionParameters);

        string ApplyConversionRule(string sourceCode, ConversionParameters conversionParameters);
    }
}
