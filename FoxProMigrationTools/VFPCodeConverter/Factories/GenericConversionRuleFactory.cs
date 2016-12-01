using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OfficeOpenXml;
using WPFLibrary.Extensions;

namespace VFPCodeConverter
{
    public class GenericConversionRuleFactory
    {
        #region Properties

        public List<GenericConversionRuleInfo> GenericConversionRuleInfoList { get; set; }

        public List<GenericConversionRule> GenericConversionRuleList { get; set; }
        #endregion

        #region Constructor

        public GenericConversionRuleFactory()
        {
            LoadRuleList();
        }
        #endregion

        #region Public Methods

        public bool ConvertLine(string sourceCodeLine, out string convertedLine)
        {
            bool isLineConverted = false;
            convertedLine = string.Empty;
            foreach (var conversionMethodModel in GenericConversionRuleInfoList)
            {
                if (conversionMethodModel.TrimLeftBeforeMatch)
                    sourceCodeLine = sourceCodeLine.TrimStart();

                if (conversionMethodModel.TrimRightBeforeMatch)
                    sourceCodeLine = sourceCodeLine.TrimEnd();

                var regex = new Regex(conversionMethodModel.RuleApplicablePattern);

                Match match = regex.Match(sourceCodeLine);
                if (match.Success)
                {
                    

                }
            }
            return isLineConverted;
        }

        public void LoadRuleList()
        {
            FileInfo newFile = new FileInfo(ConfigurationManager.AppSettings.Get("EXCEL_FILE"));

            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                var sheet = pck.Workbook.Worksheets.FirstOrDefault();

                if (sheet == null)
                    return;

                GenericConversionRuleInfoList = new List<GenericConversionRuleInfo>();


                var start = sheet.Dimension.Start;
                var end = sheet.Dimension.End;
                // Skip header row
                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    GenericConversionRuleInfo conversionMethod = new GenericConversionRuleInfo();
                    conversionMethod.RuleName = sheet.Cells[row, 1].Text;
                    conversionMethod.RuleType = sheet.Cells[row, 2].Text;
                    conversionMethod.RuleTypePriority = sheet.Cells[row, 3].Text.ConvertToInt32();
                    conversionMethod.GlobalPriority = sheet.Cells[row, 4].Text.ConvertToInt32();
                    conversionMethod.TrimLeftBeforeMatch = sheet.Cells[row, 5].Text.ConvertToBool();
                    conversionMethod.TrimRightBeforeMatch = sheet.Cells[row, 6].Text.ConvertToBool();
                    conversionMethod.RuleApplicablePattern = sheet.Cells[row, 7].Text;
                    conversionMethod.ReplacePattern = sheet.Cells[row, 8].Text;

                    GenericConversionRuleInfoList.Add(conversionMethod);
                }
            }

            GenericConversionRuleList = new List<GenericConversionRule>();

            // Create GenericConversionMethods
            foreach (var conversionMethodModel in GenericConversionRuleInfoList)
            {
                GenericConversionRuleList.Add(new GenericConversionRule(conversionMethodModel));
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
