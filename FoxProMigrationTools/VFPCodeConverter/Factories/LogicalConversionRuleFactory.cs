using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using VFPCodeConverter.ConversionMethods.Common;
using WPFLibrary.Extensions;

namespace VFPCodeConverter
{
    public class LogicalConversionRuleFactory
    {
        #region Properties

        public List<LogicalConversionRuleInfo> LogicalConversionRuleInfoList { get; set; }

        public List<IConversionRule> LogicalConversionRuleList { get; set; }
        #endregion

        #region Constructor

        public LogicalConversionRuleFactory()
        {
            LoadRuleList();
        }
        #endregion

        #region Private Methods

        private void LoadRuleList()
        {
            LogicalConversionRuleList = new List<IConversionRule>();

            var type = typeof(IConversionRule);
            List<Type> conversionRuleClassList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsInterface == false && p != typeof(GenericConversionRule)).ToList();

            foreach (Type conversionRuleClassType in conversionRuleClassList)
            {
                LogicalConversionRuleList.Add(Activator.CreateInstance(conversionRuleClassType) as IConversionRule);
            }

            LoadRuleInfoList();

            foreach (var conversionRule in LogicalConversionRuleList)
            {
                var conversionRuleBase = conversionRule as ConversionRuleBase;

                if (conversionRuleBase == null)
                {
                    System.Diagnostics.Debugger.Break();
                    continue;                    
                }

                var excelEntry = LogicalConversionRuleInfoList.FirstOrDefault(info => info.RuleName == conversionRule.RuleName);
                if (excelEntry == null)
                {
                    System.Diagnostics.Debugger.Break();
                    continue;
                }

                conversionRuleBase.RuleTypePriority = excelEntry.RuleTypePriority;
                conversionRuleBase.GlobalPriority = excelEntry.GlobalPriority;
                conversionRuleBase.RuleApplicablePattern = excelEntry.RuleApplicablePattern;
                conversionRuleBase.SubPattern = excelEntry.SubPattern;
            }
        }

        private void LoadRuleInfoList()
        {
            FileInfo newFile = new FileInfo(ConfigurationManager.AppSettings.Get("EXCEL_FILE"));

            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                var sheet = pck.Workbook.Worksheets[2];

                if (sheet == null)
                    return;

                LogicalConversionRuleInfoList = new List<LogicalConversionRuleInfo>();


                var start = sheet.Dimension.Start;
                var end = sheet.Dimension.End;
                // Skip header row
                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    LogicalConversionRuleInfo conversionMethod = new LogicalConversionRuleInfo();
                    conversionMethod.RuleName = sheet.Cells[row, 1].Text;
                    conversionMethod.RuleType = sheet.Cells[row, 2].Text;
                    conversionMethod.RuleTypePriority = sheet.Cells[row, 3].Text.ConvertToInt32();
                    conversionMethod.GlobalPriority = sheet.Cells[row, 4].Text.ConvertToInt32();
                    conversionMethod.RuleApplicablePattern = sheet.Cells[row, 5].Text;
                    conversionMethod.SubPattern = sheet.Cells[row, 7].Text;

                    LogicalConversionRuleInfoList.Add(conversionMethod);
                }
            }
        }
        #endregion
    }
}
