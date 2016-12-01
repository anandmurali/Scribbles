using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparer.Common
{
    public static class DynamicColumn
    {
        public static string GetIsColumnPresentColumnName(string columnName)
        {
            return "_Is_Col_" + columnName + "_Present";
        }

        public static string GetIsDataEqualColumnName(string columnName)
        {
            return "_Is_Data_" + columnName + "_Equal";
        }

        public static string GetIsVisibleColumnName(string columnName)
        {
            return "_Is_Col_" + columnName + "_Visible";
        }
    }
}
