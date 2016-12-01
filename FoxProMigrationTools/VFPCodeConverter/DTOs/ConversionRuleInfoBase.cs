using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFPCodeConverter
{
    public class ConversionRuleInfoBase
    {
        #region Properties

        public string RuleName { get; set; }

        public string RuleType { get; set; }

        public int RuleTypePriority { get; set; }

        public int GlobalPriority { get; set; }

        public string RuleApplicablePattern { get; set; }

        public string SubPattern { get; set; }
        #endregion
    }
}
