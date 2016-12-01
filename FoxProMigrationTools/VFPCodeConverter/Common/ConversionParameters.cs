using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFPCodeConverter.CodeBuilders;
using VFPCodeConverter.DTOs;

namespace VFPCodeConverter
{
    public class ConversionParameters
    {
        #region Properties

        public SourceCodeType SourceCodeType { get; set; }

        public List<string> ConvertedCodeLines { get; set; }

        public bool CopyComments { get; set; }

        public bool IsLastCharacterType { get; set; }

        public bool IsLocalVariableGroupingRequired { get; set; }

        public MethodBuilder MethodBuilder { get; set; }

        public VoidMethodBuilder VoidMethodBuilder { get; set; }
        #endregion

        #region Constructor

        public ConversionParameters()
        {
            ConvertedCodeLines = new List<string>();

            CopyComments = true;
            IsLastCharacterType = true;
            IsLocalVariableGroupingRequired = true;
        }
        #endregion


        #region Methods

        public void AddConvertedCode(string convertedCode)
        {
            switch (SourceCodeType)
            {
                case SourceCodeType.File:
                    break;
                case SourceCodeType.Method:
                    if (MethodBuilder != null)
                        MethodBuilder.LinesOfCode.Add(convertedCode);
                    else
                        VoidMethodBuilder.LinesOfCode.Add(convertedCode);
                    break;
                case SourceCodeType.LinesOfCode:
                    ConvertedCodeLines.Add(convertedCode);
                    break;
            }
        }

        public void AddMethodName(string leadingSpace, string methodName, string inlineComments)
        {
            switch (SourceCodeType)
            {
                case SourceCodeType.File:
                    break;
                case SourceCodeType.Method:
                    if (MethodBuilder != null)
                    {
                        MethodBuilder.LeadingSpace = leadingSpace;
                        MethodBuilder.MethodName = methodName;
                        MethodBuilder.InLineComments = inlineComments;
                    }
                    else
                    {
                        VoidMethodBuilder.LeadingSpace = leadingSpace;
                        VoidMethodBuilder.MethodName = methodName;
                        VoidMethodBuilder.InLineComments = inlineComments;
                    }
                    break;
            }
        }

        public void AddOptionalParameterValue(string paramterName, string defaultValue, int paramterCount)
        {
            switch (SourceCodeType)
            {
                case SourceCodeType.File:
                    break;
                case SourceCodeType.Method:
                    MethodBuilderBase methodBuilderBase = null;
                    if (MethodBuilder != null)
                    {
                        methodBuilderBase = MethodBuilder;
                    }
                    else
                    {
                        methodBuilderBase = VoidMethodBuilder;
                    }
                    MethodParameterBuilder.AddOptionalParamtervalue(methodBuilderBase.Parameters, paramterName, defaultValue, paramterCount);
                    break;
            }
        }

        public void AddMethodParameters(string dataType, string parameterName)
        {
            switch (SourceCodeType)
            {
                case SourceCodeType.File:
                    break;
                case SourceCodeType.Method:
                    if (MethodBuilder != null)
                        MethodBuilder.Parameters.Add(MethodParameterBuilder.GetMethodParameterInfo(parameterName, dataType));
                    else
                        VoidMethodBuilder.Parameters.Add(MethodParameterBuilder.GetMethodParameterInfo(parameterName, dataType));
                    break;
            }
        }

        public string GetConvertedCode()
        {
            switch (SourceCodeType)
            {
                case SourceCodeType.File:
                    break;
                case SourceCodeType.LinesOfCode:
                    StringBuilder codeStringBuilder = new StringBuilder();
                    ConvertCodeLines(codeStringBuilder, ConvertedCodeLines);
                    return codeStringBuilder.ToString();
                case SourceCodeType.Method:
                    StringBuilder methodStringBuilder = new StringBuilder();
                    MethodBuilderBase methodBuilderBase = null;
                    if (VoidMethodBuilder != null)
                    {
                        methodStringBuilder.Append(VoidMethodBuilder.LeadingSpace);
                        methodStringBuilder.Append("public void " + VoidMethodBuilder.MethodName);
                        methodBuilderBase = VoidMethodBuilder;
                    }
                    else
                    {
                        methodStringBuilder.Append(MethodBuilder.LeadingSpace);
                        methodStringBuilder.Append("public object " + MethodBuilder.MethodName);
                        methodBuilderBase = MethodBuilder;
                    }
                    MethodParameterBuilder.FillConvertedCode(methodStringBuilder, methodBuilderBase.Parameters);

                    if (!string.IsNullOrEmpty(methodBuilderBase.InLineComments))
                    {
                        methodStringBuilder.AppendLine(" \\\\ " +methodBuilderBase.InLineComments);
                    }

                    methodStringBuilder.AppendLine(methodBuilderBase.LeadingSpace + "{");
                    ConvertCodeLines(methodStringBuilder, methodBuilderBase.LinesOfCode);
                    methodStringBuilder.AppendLine(Environment.NewLine + methodBuilderBase.LeadingSpace + "}");
                    return methodStringBuilder.ToString();
                    break;
            }
            return string.Empty;
        }

        private void ConvertCodeLines(StringBuilder codeStringBuilder, List<string> linesofCode )
        {
            foreach (var convertedCodeLine in linesofCode)
            {
                switch (convertedCodeLine)
                {
                    case "\\r\\n":
                        codeStringBuilder.AppendLine();
                        break;
                    default:
                        codeStringBuilder.Append(convertedCodeLine.Replace("\\r\\n", "\r\n"));
                        break;
                }
            }
        }

        #endregion
    }

    public enum SourceCodeType
    {
        File, // Multiple Methods
        Method, // Single Procedure Or Function
        LinesOfCode // No Procedure Or Function
    }
}
