using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataComparer.Common;
using DataComparer.Common.Contracts;
using DataComparer.Common.Domain;
using WPFLibrary.Extensions;

namespace DataComparer.Dal
{
    public class DataTableComparer : IDataTableComparer
    {
        #region IDataTableComparer Implementation
        public List<RowComparison> Compare(TableDetail tableDetail, DataTable firstDataTable, DataTable secondDataTable)
        {
            List<RowComparison> rowComparisonList = new List<RowComparison>();


            if (firstDataTable != null)
            {
                int loopCount = 0;
                foreach (DataRow firstDbDataRow in firstDataTable.Rows)
                {
                    var firstDbPrimaryColumnValue = firstDbDataRow[tableDetail.PrimaryColumnName].ConvertToString();

                    RowComparison rowComparsion = new RowComparison();
                    rowComparsion.FirstDatabasePrimaryColumnValue = firstDbPrimaryColumnValue;
                    rowComparsion.FirstDbDataRow = firstDbDataRow;

                    DataRow secondDbDataRow = null;
                    if (secondDataTable != null)
                    {
                        // Check if same unique value exists in second db
                        secondDbDataRow = GetDataRow(firstDbDataRow, secondDataTable, tableDetail.CompareColumnList);

                        if (secondDbDataRow == null && loopCount < secondDataTable.Rows.Count)
                        {
                            secondDbDataRow = secondDataTable.Rows[loopCount];
                        }

                        if (secondDbDataRow != null)
                            rowComparsion.SecondDatabasePrimaryColumnValue = secondDbDataRow[tableDetail.PrimaryColumnName].ConvertToString();

                        List<string> uniqueValuesInSecondDb = new List<string>();
                        foreach (DataRow dataRow in secondDataTable.Rows)
                        {
                            uniqueValuesInSecondDb.Add(dataRow[tableDetail.PrimaryColumnName].ConvertToString());
                        }
                        rowComparsion.SecondDatabasePrimaryColumnValueList = uniqueValuesInSecondDb;
                    }

                    rowComparsion.ResultDataTable = Compare(GetAllColumns(firstDataTable, secondDataTable), firstDbDataRow, secondDbDataRow);

                    rowComparisonList.Add(rowComparsion);
                    loopCount++;
                }
            }

            return rowComparisonList;
        }

        public DataRow GetDataRow(DataTable dataTable, string uniqueColumnName, string uniqueColumnValue)
        {
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRow[uniqueColumnName].ConvertToString() == uniqueColumnValue)
                    return dataRow;
            }
            return null;
        }

        public DataRow GetDataRow(DataRow firstDbDataRow, DataTable secondDbDataTable, List<string> columnComparisonList)
        {
            foreach (DataRow dataRow in secondDbDataTable.Rows)
            {
                bool isAllColumnValueSame = true;
                foreach (var columnName in columnComparisonList)
                {
                    var columnDataType = GetColumnDataType(columnName, firstDbDataRow.Table, secondDbDataTable);
                    if (!CompareCellValues(firstDbDataRow[columnName].ConvertToString(), dataRow[columnName].ConvertToString(), columnDataType))
                    {
                        isAllColumnValueSame = false;
                        break;
                    }
                }
                if (isAllColumnValueSame)
                    return dataRow;
            }
            return null;
        }

        public void Compare(TableDetail tableDetail, RowComparison rowComparison, DataRow firsDbDataRow, DataRow secondDbDataRow)
        {
            var allColumnDictonary = GetAllColumns(tableDetail.FirstDatabaseData, tableDetail.SecondDatabaseData);
            rowComparison.ResultDataTable = Compare(allColumnDictonary, firsDbDataRow, secondDbDataRow);
        }

        public DataTable Compare(DataRow firstDbDataRow, DataRow secondDbDataRow)
        {
            var allColumnDictionary = GetAllColumns(firstDbDataRow.Table, secondDbDataRow.Table);
            return Compare(allColumnDictionary, firstDbDataRow, secondDbDataRow);
        }

        public DataTable GetDifferenceDataTable(DataTable firstDataTable, DataTable secondDataTable, string[] uniqueColumns)
        {
            DataTable firstMergedDataTable;
            DataTable secondMergedDataTable;
            MergeDataTableColumns(firstDataTable, secondDataTable, out firstMergedDataTable, out secondMergedDataTable);

            var allColumnDictionary = GetAllColumns(firstDataTable, secondDataTable);
            DataTable differenceDataTable = firstMergedDataTable.Clone();

            List<DataRow> rowsToDelete = new List<DataRow>();
            foreach (DataRow dataRow in firstMergedDataTable.Rows)
            {
                
                StringBuilder stringBuilder = new StringBuilder();
                int i = 0;
                foreach (var uniqueColumn in uniqueColumns)
                {
                    if (i == 0)
                        stringBuilder.Append(uniqueColumn + "'" + dataRow[uniqueColumn] + "' ");
                    else
                        stringBuilder.Append(" AND " + uniqueColumn + "'" + dataRow[uniqueColumn] + "' ");
                    i++;
                }

                var dataRows = secondMergedDataTable.Select(stringBuilder.ToString());

                if (dataRows != null && dataRows.Count() > 0)
                {
                    var result = Compare(allColumnDictionary, dataRow, dataRows[0]);
                    foreach (DataRow row in result.Rows)
                    {
                        differenceDataTable.Rows.Add(row.ItemArray);
                    }
                    secondMergedDataTable.Rows.Remove(dataRows[0]);
                }
                else
                {
                    differenceDataTable.Rows.Add(dataRow.ItemArray);
                }
                rowsToDelete.Add(dataRow);
            }

            foreach (var dataRow in rowsToDelete)
            {
                firstMergedDataTable.Rows.Remove(dataRow);
            }

            foreach (DataRow dataRow in secondMergedDataTable.Rows)
            {
                StringBuilder stringBuilder = new StringBuilder();
                int i = 0;
                foreach (var uniqueColumn in uniqueColumns)
                {
                    if (i == 0)
                        stringBuilder.Append(uniqueColumn + "'" + dataRow[uniqueColumn] + "' ");
                    else
                        stringBuilder.Append(" AND " + uniqueColumn + "'" + dataRow[uniqueColumn] + "' ");
                    i++;
                }

                var dataRows = firstMergedDataTable.Select(stringBuilder.ToString());

                if (dataRows != null && dataRows.Count() > 0)
                {
                    var result = Compare(allColumnDictionary, dataRow, dataRows[0]);
                    foreach (DataRow row in result.Rows)
                    {
                        differenceDataTable.Rows.Add(row.ItemArray);
                    }
                    firstMergedDataTable.Rows.Remove(dataRows[0]);
                }
                else
                {
                    differenceDataTable.Rows.Add(dataRow.ItemArray);
                }
                rowsToDelete.Add(dataRow);
            }

            return differenceDataTable;
        }

        public bool MergeDataTableColumns(DataTable firstDbData, DataTable secondDbData, out DataTable firstDbMergedData, out DataTable secondDbMergedData)
        {

            firstDbMergedData = null;
            secondDbMergedData = null;

            if (firstDbData == null && secondDbData == null)
                return false;

            var allColumns = GetAllColumns(firstDbData, secondDbData);

            firstDbMergedData = new DataTable();
            secondDbMergedData = new DataTable();

            AddColumnsInDataTable(allColumns, firstDbMergedData);
            AddColumnsInDataTable(allColumns, secondDbMergedData);

            AddRowsToDataTable(firstDbData, firstDbMergedData);
            AddRowsToDataTable(secondDbData, secondDbMergedData);

            return true;
        }
        #endregion

        #region Private Methods

        private Type GetColumnDataType(string columnName, DataTable firstDbData, DataTable secondDbData)
        {
            foreach (var dataColumn in firstDbData.Columns.Cast<DataColumn>())
            {
                if (columnName.ToUpper() == dataColumn.ColumnName.ToUpper())
                {
                    return dataColumn.DataType;
                }
            }
            foreach (var dataColumn in secondDbData.Columns.Cast<DataColumn>())
            {
                if (columnName.ToUpper() == dataColumn.ColumnName.ToUpper())
                {
                    return dataColumn.DataType;
                }
            }
            return null;
        }

        private SortedDictionary<string, CellComparison> GetAllColumns(DataTable firstDataTable, DataTable secondDataTable)
        {
            var allColumnDictionary = new SortedDictionary<string, CellComparison>();

            if (firstDataTable != null)
            {
                foreach (var dataColumn in firstDataTable.Columns.Cast<DataColumn>())
                {
                    CellComparison columnComparisonDetail = new CellComparison();
                    columnComparisonDetail.IsColumnAvailableInFirstDatabase = true;
                    columnComparisonDetail.ColumnName = dataColumn.ColumnName.ToUpper();
                    columnComparisonDetail.DataType = dataColumn.DataType;

                    allColumnDictionary.Add(dataColumn.ColumnName.ToUpper(), columnComparisonDetail);
                }
            }
            if (secondDataTable != null)
            {
                foreach (var dataColumn in secondDataTable.Columns.Cast<DataColumn>())
                {
                    CellComparison columnComparisonDetail = null;
                    if (allColumnDictionary.TryGetValue(dataColumn.ColumnName.ToUpper(), out columnComparisonDetail))
                    {
                        columnComparisonDetail.IsColumnAvailableInSecondDatabase = true;
                    }
                    else
                    {
                        columnComparisonDetail = new CellComparison();
                        columnComparisonDetail.FirstDatabaseColumnValue = "--NA--";
                        columnComparisonDetail.IsColumnAvailableInFirstDatabase = false;
                        columnComparisonDetail.IsColumnAvailableInSecondDatabase = true;
                        columnComparisonDetail.ColumnName = dataColumn.ColumnName.ToUpper();
                        columnComparisonDetail.DataType = dataColumn.DataType;

                        allColumnDictionary.Add(dataColumn.ColumnName.ToUpper(), columnComparisonDetail);
                    }
                }
            }
            return allColumnDictionary;
        }

        private DataTable Compare(SortedDictionary<string, CellComparison> allColumnDictionary, DataRow firstDbDataRow, DataRow secondDbDataRow)
        {
            if (firstDbDataRow != null)
            {
                foreach (var cellComparison in allColumnDictionary.Values.Where(cell => cell.IsColumnAvailableInFirstDatabase))
                {
                    cellComparison.FirstDatabaseColumnValue = firstDbDataRow[cellComparison.ColumnName].ConvertToString();
                }
            }

            if (secondDbDataRow != null)
            {
                foreach (var cellComparison in allColumnDictionary.Values.Where(cell => cell.IsColumnAvailableInSecondDatabase))
                {
                    cellComparison.SecondDatabaseColumnValue = secondDbDataRow[cellComparison.ColumnName].ConvertToString();
                    cellComparison.IsDataEqual = CompareCellValues(cellComparison.FirstDatabaseColumnValue, cellComparison.SecondDatabaseColumnValue, cellComparison.DataType);
                }
            }

            return ConvertToDataTable(allColumnDictionary);
        }

        private DataTable ConvertToDataTable(SortedDictionary<string, CellComparison> allColumnDictionary)
        {
            DataTable dataTable = new DataTable();

            var column = new DataColumn(Constants.DatabaseTypeColumnName);
            dataTable.Columns.Add(column);

            // Add Columns
            foreach (var cellComparison in allColumnDictionary)
            {
                column = new DataColumn(cellComparison.Value.ColumnName);
                dataTable.Columns.Add(column);

                column = new DataColumn(DynamicColumn.GetIsColumnPresentColumnName(cellComparison.Value.ColumnName));
                column.DataType = typeof(bool);
                dataTable.Columns.Add(column);

                column = new DataColumn(DynamicColumn.GetIsDataEqualColumnName(cellComparison.Value.ColumnName));
                column.DataType = typeof(bool);
                dataTable.Columns.Add(column);

                column = new DataColumn(DynamicColumn.GetIsVisibleColumnName(cellComparison.Value.ColumnName));
                column.DataType = typeof(bool);
                dataTable.Columns.Add(column);
            }

            // Add Rows
            DataRow firstDbDataRow = dataTable.NewRow();
            DataRow secondDbDataRow = dataTable.NewRow();

            foreach (var columnComparisonDetail in allColumnDictionary)
            {
                var columnName = columnComparisonDetail.Value.ColumnName;
                // DATABASE
                firstDbDataRow[Constants.DatabaseTypeColumnName] = ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseOneType];
                secondDbDataRow[Constants.DatabaseTypeColumnName] = ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseTwoType]; ;

                // Value
                firstDbDataRow[columnName] = columnComparisonDetail.Value.FirstDatabaseColumnValue;
                secondDbDataRow[columnName] = columnComparisonDetail.Value.SecondDatabaseColumnValue;

                // IsColumnPresent
                firstDbDataRow[DynamicColumn.GetIsColumnPresentColumnName(columnName)] = columnComparisonDetail.Value.IsColumnAvailableInFirstDatabase;
                secondDbDataRow[DynamicColumn.GetIsColumnPresentColumnName(columnName)] = columnComparisonDetail.Value.IsColumnAvailableInSecondDatabase;

                // IsDataEqual
                firstDbDataRow[DynamicColumn.GetIsDataEqualColumnName(columnName)] = columnComparisonDetail.Value.IsDataEqual;
                secondDbDataRow[DynamicColumn.GetIsDataEqualColumnName(columnName)] = columnComparisonDetail.Value.IsDataEqual;

                // IsVisible
                firstDbDataRow[DynamicColumn.GetIsVisibleColumnName(columnName)] = true;
                secondDbDataRow[DynamicColumn.GetIsVisibleColumnName(columnName)] = true;
            }

            dataTable.Rows.Add(firstDbDataRow);
            dataTable.Rows.Add(secondDbDataRow);

            return dataTable;
        }

        private bool CompareCellValues(string firstColumnValue, string secondColumnValue, Type dataType)
        {
            if (string.IsNullOrEmpty(firstColumnValue) && string.IsNullOrEmpty(secondColumnValue))
            {
                return true;
            }

            if (ConfigurationManager.AppSettings[Constants.AppSettingKeyTrimspaces].ConvertToBool())
            {
                var tempFirstColumnValue = firstColumnValue;
                var tempSecondColumnValue = secondColumnValue;
                if (tempFirstColumnValue != null)
                    tempFirstColumnValue = tempFirstColumnValue.Trim();
                if (tempSecondColumnValue != null)
                    tempSecondColumnValue = tempSecondColumnValue.Trim();

                if (string.IsNullOrEmpty(tempFirstColumnValue) && string.IsNullOrEmpty(tempSecondColumnValue))
                {
                    return true;
                }
                else
                {
                    if (dataType != null)
                    {
                        if (dataType == typeof(DateTime))
                        {
                            var nullDataValues = GetFormattedDate(ConfigurationManager.AppSettings[Constants.AppSettingKeyNullDataValues].ConvertToString());
                            if (!string.IsNullOrWhiteSpace(nullDataValues))
                            {
                                if (tempFirstColumnValue != null)
                                    tempFirstColumnValue = nullDataValues.Contains(tempFirstColumnValue) ? null : tempFirstColumnValue;

                                if (tempSecondColumnValue != null)
                                    tempSecondColumnValue = nullDataValues.Contains(tempSecondColumnValue) ? null : tempSecondColumnValue;
                            }
                        }
                    }
                    return tempFirstColumnValue == tempSecondColumnValue;
                }
            }

            return firstColumnValue == secondColumnValue;
        }

        private void AddColumnsInDataTable(SortedDictionary<string, CellComparison> allColumns, DataTable dataTable)
        {
            foreach (var cellComparison in allColumns)
            {
                if (cellComparison.Value.IsColumnAvailableInFirstDatabase && cellComparison.Value.IsColumnAvailableInSecondDatabase)
                    dataTable.Columns.Add(cellComparison.Value.ColumnName, cellComparison.Value.DataType);
                else
                    dataTable.Columns.Add(cellComparison.Value.ColumnName);
            }
        }

        private string GetFormattedDate(string unFormattedDateTimeString)
        {
            if (string.IsNullOrEmpty(unFormattedDateTimeString))
                return null;

            List<string> dateTimeStrList = unFormattedDateTimeString.Split(';').ToList();

            StringBuilder formattedString = new StringBuilder();
            foreach (var dateTimeStr in dateTimeStrList)
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(dateTimeStr, "dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    formattedString.Append(dateTime.ToString());
                    formattedString.Append(";");
                }
            }
            return formattedString.ToString();
        }

        private void AddRowsToDataTable(DataTable fromDataTable, DataTable toDataTable)
        {

            List<string> columnsNotInFromDataTable = new List<string>();

            foreach (DataColumn secondDtColumn in toDataTable.Columns)
            {
                bool isColumnPresent = false;
                foreach (DataColumn firstDtColumn in fromDataTable.Columns)
                {
                    if (firstDtColumn.ColumnName.ToLower() == secondDtColumn.ColumnName.ToLower())
                    {
                        isColumnPresent = true;
                        break;
                    }
                }

                if (isColumnPresent == false)
                    columnsNotInFromDataTable.Add(secondDtColumn.ColumnName);
            }

            foreach (DataRow row in fromDataTable.Rows)
            {
                var newRow = toDataTable.NewRow();
                foreach (DataColumn column in fromDataTable.Columns)
                {
                    newRow[column.ColumnName] = row[column.ColumnName];
                }
                toDataTable.Rows.Add(newRow);


            }

            foreach (DataRow row in toDataTable.Rows)
            {
                foreach (var columnName in columnsNotInFromDataTable)
                {
                    row[columnName] = "--N/A--";
                }
            }
        }
        #endregion
    }
}
