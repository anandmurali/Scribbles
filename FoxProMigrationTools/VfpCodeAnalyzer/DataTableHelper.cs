using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public class DataTableHelper
    {
        public static DataColumn GetAutoIncrementColumn(string columnName)
        {
            DataColumn dataColumn = new DataColumn();
            dataColumn.ColumnName = columnName;
            dataColumn.DataType = typeof(Int32);
            dataColumn.AutoIncrement = true;
            dataColumn.AutoIncrementSeed = 1;
            dataColumn.AutoIncrementStep = 1;
            return dataColumn;
        }
    }
}
