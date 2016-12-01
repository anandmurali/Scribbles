using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataComparer.Common.Domain;

namespace DataComparer.Common.Contracts
{
    public interface IDataTableComparer
    {
        List<RowComparison> Compare(TableDetail tableDetail, DataTable firstDataTable, DataTable secondDataTable);

        void Compare(TableDetail tableDetail, RowComparison rowComparison, DataRow firsDbDataRow, DataRow secondDbDataRow);

        DataRow GetDataRow(DataRow firstDbDataRow, DataTable secondDbDataTable, List<string> columnComparisonList);

        DataRow GetDataRow(DataTable dataTable, string uniqueColumnName, string uniqueColumnValue);

        bool MergeDataTableColumns(DataTable firstDbData, DataTable secondDbData, out DataTable firstDbMergedData, out DataTable secondDbMergedData);

        DataTable Compare(DataRow firstDbDataRow, DataRow secondDbDataRow);

        DataTable GetDifferenceDataTable(DataTable firstDataTable, DataTable secondDataTable, string[] uniqueColumns);
    }
}
