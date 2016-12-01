using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VFPCodeConverter.CodeBuilders;
using VFPCodeConverter.Common;

namespace VFPCodeConverter
{
    public class CodeConverter
    {
        #region Properties

        public List<IConversionRule> ConversionRules { get; set; }

        #endregion

        #region Constructor

        public CodeConverter()
        {
            ConversionRules = new List<IConversionRule>();
        }
        #endregion

        #region Public Methods

        public void Initialize()
        {
            var genericConversionCreator = new GenericConversionRuleFactory();
            ConversionRules.AddRange(genericConversionCreator.GenericConversionRuleList);

            var logicalConversionRuleFactory = new LogicalConversionRuleFactory();
            ConversionRules.AddRange(logicalConversionRuleFactory.LogicalConversionRuleList);

            ConversionRules = ConversionRules.OrderBy(rule => rule.GlobalPriority).ThenBy(rule => rule.RuleType).ThenBy(rule => rule.RuleTypePriority).ToList();
        }


        public string Convert(string sourceCode)
        {
            var conversionParamters = GetConversionParamters(sourceCode);
            int loopCount = 0;

            string previousSourceCode = null;
            while (!string.IsNullOrEmpty(sourceCode))
            {
                loopCount++;
                Logger.AddLog("Loop count " + loopCount);

                foreach (var conversionRule in ConversionRules)
                {
                    if (conversionRule.IsRuleApplicable(sourceCode, conversionParamters))
                    {
                        sourceCode = conversionRule.ApplyConversionRule(sourceCode, conversionParamters);
                        break;
                    }
                }

                if (previousSourceCode == sourceCode)
                {
                    if (sourceCode == null)
                        break;

                    var match = Regex.Match(sourceCode, RegexManger.FirstLinePattern);
                    if (match.Success)
                    {
                        if (match.Index == 0 && match.Length == 0)
                        {
                            // Handle Empty Match
                            sourceCode = sourceCode.Remove(0, 1);
                        }
                        else
                        {
                            //conversionParamters.AddConvertedCode("\\\\ TODO\n\\\\ " + match.Value);
                            conversionParamters.AddConvertedCode("\\\\ " + match.Value);
                            sourceCode = sourceCode.Remove(match.Index, match.Length);
                        }

                        if (previousSourceCode == sourceCode)
                            System.Diagnostics.Debugger.Break();
                    }
                }

                previousSourceCode = sourceCode;
            }
            return conversionParamters.GetConvertedCode();
        }
        #endregion

        #region Private Methods

        private ConversionParameters GetConversionParamters(string sourceCode)
        {
            ConversionParameters conversionParameters = new ConversionParameters();

            var regex = new Regex(RegexManger.NoOfMethodsPattern);
            var matches = regex.Matches(sourceCode);
            if (matches.Count == 0)
                conversionParameters.SourceCodeType = SourceCodeType.LinesOfCode;
            else if (matches.Count == 1)
            {
                conversionParameters.SourceCodeType = SourceCodeType.Method;
                if (Regex.IsMatch(sourceCode, RegexManger.ProcedurePattern))
                {
                    conversionParameters.VoidMethodBuilder = new VoidMethodBuilder();
                }
                else
                {
                    conversionParameters.MethodBuilder = new MethodBuilder();
                }
            }
            else
                conversionParameters.SourceCodeType = SourceCodeType.File;

            return conversionParameters;
        }
        #endregion
    }
}
