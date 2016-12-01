using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFPCodeConverter.Common
{
    public static class Logger
    {
        #region Fields

        private static readonly StringBuilder _logBuiler = new StringBuilder(); 
        #endregion

        #region Methods

        public static void AddLog(string log)
        {
            _logBuiler.AppendLine(log);
        }

        public static string GetLog()
        {
            return _logBuiler.ToString();
        }

        #endregion

        #region ApplyConversionRule Logs

        public static void AddApplyConversionRuleEntryLog(string ruleName, string ruleType, int ruleTypePriority)
        {
            AddLog(String.Format("ApplyingRule Name: {0}-Type: {1}-Priority: {2}", ruleName, ruleType, ruleTypePriority));
        }

        public static void AddSourceCode(string sourceCode)
        {
            AddLog(String.Format("SourceCode: {0}", sourceCode));
        }

        public static void AddConvertedCode(string sourceCode)
        {
            AddLog(String.Format("ConvertedCode: {0}", sourceCode));
        }
        #endregion
    }
}
