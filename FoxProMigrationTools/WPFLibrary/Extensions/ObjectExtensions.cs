using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFLibrary.Extensions
{
    public static class ObjectExtensions
    {
        public static string ConvertToString(this object obj)
        {
            if (obj == null || obj is DBNull)
                return null;
            else
            {
                return obj.ToString();
            }
        }

        public static Int32 ConvertToInt32(this object objectParam)
        {
            if (objectParam == null)
                return default(Int32);

            Int32 convertedValue;
            if (Int32.TryParse(objectParam.ToString(), out convertedValue))
                return convertedValue;
            return default(Int32);
        }

        public static bool ConvertToBool(this object obj)
        {
            if (obj == null || obj is DBNull)
                return false;
            else
            {
                if (obj.ToString() == "1")
                    return true;

                if (obj.ToString() == "0")
                    return false;

                return Convert.ToBoolean(obj);
            }
        }

        public static DateTime? ConvertToDateTime(this object objectParam)
        {
            if (objectParam == null || objectParam == DBNull.Value)
                return null;

            return Convert.ToDateTime(objectParam);
        }
    }
}
