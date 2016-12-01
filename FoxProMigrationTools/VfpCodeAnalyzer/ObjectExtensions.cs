using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Nullproof implementation of ToString()
        /// </summary>
        /// <param name="objectParam"></param>
        /// <returns></returns>
        public static string ConvertToString(this object objectParam)
        {
            if (objectParam == null || objectParam is DBNull)
                return null;

            return objectParam.ToString();
        }

        /// <summary>
        /// If object is null then 0 is returned. If object has a valid Int32 value then it is returned else 
        /// 0 is returned.
        /// </summary>
        /// <param name="objectParam"></param>
        /// <returns></returns>
        public static Int32 ConvertToInt32(this object objectParam)
        {
            if (objectParam == null)
                return default(Int32);

            Int32 convertedValue;
            if (Int32.TryParse(objectParam.ToString(), out convertedValue))
                return convertedValue;
            return default(Int32);
        }
    }
}
