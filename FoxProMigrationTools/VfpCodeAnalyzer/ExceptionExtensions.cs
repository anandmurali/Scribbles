using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public static class ExceptionExtensions
    {
        public static String ToLogString(this Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ExceptionToString(stringBuilder, exception);
            return stringBuilder.ToString();
        }

        private static void ExceptionToString(StringBuilder stringBuilder, Exception exception)
        {
            stringBuilder.AppendLine(exception.GetType().ToString());
            stringBuilder.AppendLine("Message");
            stringBuilder.AppendLine(exception.Message);
            stringBuilder.AppendLine("StackTrace");
            stringBuilder.AppendLine(exception.StackTrace);

            if (exception.InnerException != null)
            {
                stringBuilder.AppendLine("Inner Exception");
                ExceptionToString(stringBuilder, exception.InnerException);
            }
        }
    }
}
