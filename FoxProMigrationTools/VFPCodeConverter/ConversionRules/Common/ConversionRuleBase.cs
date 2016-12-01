using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFPCodeConverter.ConversionMethods.Common
{
    public class ConversionRuleBase
    {
        #region Properties
        public string RuleName { get; set; }

        public int GlobalPriority { get; set; }

        public string RuleType { get; set; }

        public int RuleTypePriority { get; set; }

        public string RuleApplicablePattern { get; set; }

        public string SubPattern { get; set; }
        #endregion

        #region Constructor

        public ConversionRuleBase()
        {
            GlobalPriority = 100;
            RuleTypePriority = 100;
        }
        #endregion

        #region Public Methods

        public virtual bool IsPatternMatching(string pattern, string input)
        {
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        #endregion

        #region Overriden Methods

        public override string ToString()
        {
            if (RuleName != null)
                return RuleName;

            return base.ToString();
        }

        #endregion
    }
}
