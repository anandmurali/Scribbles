using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFPCodeConverter.DTOs;

namespace VFPCodeConverter.CodeBuilders
{
    public class MethodParameterBuilder
    {
        #region Public Static Methods
        public static void FillConvertedCode(StringBuilder methodStringBuilder, List<MethodParameterInfo> methodParameterList)
        {
            if (methodParameterList.Count == 0)
            {
                methodStringBuilder.Append("()");
            }
            else
            {
                methodStringBuilder.Append("(");
                int totalParametersCount = methodParameterList.Count;
                int loopCount = 0;
                foreach (MethodParameterInfo parameterInfo in methodParameterList)
                {
                    loopCount++;
                    if (loopCount == totalParametersCount)
                    {
                        if (parameterInfo.IsOptional)
                            methodStringBuilder.Append(GetParamStringWithOptional(parameterInfo));
                        else
                            methodStringBuilder.Append(GetParamString(parameterInfo));
                    }
                    else
                    {
                        if (parameterInfo.IsOptional)
                            methodStringBuilder.Append(GetParamStringWithOptional(parameterInfo) + ", ");
                        else
                            methodStringBuilder.Append(GetParamString(parameterInfo) + ", ");
                    }
                }
                methodStringBuilder.Append(")");
            }
            methodStringBuilder.AppendLine();
        }

        public static MethodParameterInfo GetMethodParameterInfo(string parameterName, string dataType)
        {
            return new MethodParameterInfo() {Name = parameterName, DataType = dataType};
        }

        public static bool AddOptionalParamtervalue(List<MethodParameterInfo> methodParameterList, string paramterName, string defaultValue, int paramterCount)
        {
            int totalParamterCount = methodParameterList.Count;
            if (totalParamterCount < paramterCount)
                return false;

            var optionalParamterInfo = methodParameterList[paramterCount];

            if (optionalParamterInfo.Name.Trim().ToLower() == paramterName.Trim().ToLower())
            {
                optionalParamterInfo.IsOptional = true;
                optionalParamterInfo.OptionalValue = defaultValue;
            }
            else
            {
                // Probably bug in code
                System.Diagnostics.Debugger.Break();
            }

            return true;
        }
        #endregion

        #region Private Static Methods
        private static string GetParamStringWithOptional(MethodParameterInfo parameterInfo)
        {
            return parameterInfo.Name + " " + parameterInfo.DataType + " = " + parameterInfo.OptionalValue;
        }

        private static string GetParamString(MethodParameterInfo parameterInfo)
        {
            return parameterInfo.Name + " " + parameterInfo.DataType ;
        }
        #endregion
    }
}
