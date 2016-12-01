using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLibrary.Extensions;

namespace VFPCodeConverter.Common
{
    public static class Utility
    {
        public static string GetType(string variableName)
        {
            switch (variableName.ToLower().GetLast(1))
            {
                case "l":
                    return "bool";
                case "i":
                    return "int";
                case "d":
                    return "DateTime";
                case "f":
                    return "double";
                case "s":
                    return "string";
                default:
                    return "object";
            }
        }

        public static void ConvertVariables(ConversionParameters conversionParameters, Dictionary<string, string> groupBuilder, StringBuilder convertedCodeBuilder, string variableName, string leadingSpace, bool isList)
        {
            string dataType = null;
            if (conversionParameters.IsLastCharacterType)
            {
                string typeString = Utility.GetType(variableName);
                dataType = isList ? "List<" + typeString + ">" : typeString;
            }
            else
            {
                dataType = isList ? "List<object>" : "object";
            }
            if (conversionParameters.IsLocalVariableGroupingRequired)
            {
                AddToGroupBuilder(groupBuilder, dataType, variableName);
            }
            else
                convertedCodeBuilder.AppendLine(leadingSpace + dataType + " " + variableName + ";");


        }

        public static void AddToGroupBuilder(Dictionary<string, string> groupBuilder, string dataType, string variableName)
        {
            if (groupBuilder.ContainsKey(dataType))
            {
                groupBuilder[dataType] = groupBuilder[dataType] + ", " + variableName;
            }
            else
            {
                groupBuilder.Add(dataType, variableName);
            }
        }
    }
}
