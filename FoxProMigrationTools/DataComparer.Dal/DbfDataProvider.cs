using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataComparer.Common;
using DataComparer.Common.Contracts;
using DataComparer.Common.Domain;
using WPFLibrary.Extensions;

namespace DataComparer.Dal
{
    public class DbfDataProvider : DataProviderBase, IDataProvider
    {
        #region IDataProvider Implementation
        public DataTable GetDataTable(string appSettingsConnectionStringName, TableDetail tableDetail)
        {
            if (base.IsArgumentsValid(appSettingsConnectionStringName, tableDetail))
            {
                using (OleDbConnection oleDbConnection = new OleDbConnection(ConfigurationManager.ConnectionStrings[appSettingsConnectionStringName].ConnectionString))
                {
                    // Open the connection
                    oleDbConnection.Open();

                    if (oleDbConnection.State == ConnectionState.Open)
                    {
                        string schemaQuery = "SELECT TOP 1 * FROM " + tableDetail.TableName;

                        OleDbCommand oleDbCommand = new OleDbCommand(schemaQuery, oleDbConnection);

                        OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);

                        DataSet schemaDataTable = new DataSet();
                        oleDbDataAdapter.FillSchema(schemaDataTable, SchemaType.Source);

                        var newQuery = BuildQuery(schemaDataTable.Tables[0].Columns, tableDetail.TableName)
                            + GetWhereClause(tableDetail.WhereClauseConditions, Constants.AppConfigFirstDatabaseConnectionStringName == appSettingsConnectionStringName);

                        // Dispose OleDbCommand and OleDbDataAdapter 
                        oleDbCommand.Dispose();
                        oleDbDataAdapter.Dispose();

                        oleDbCommand = new OleDbCommand(newQuery, oleDbConnection);
                        oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);

                        DataTable dataTable = new DataTable();
                        oleDbDataAdapter.Fill(dataTable);

                        // Dispose OleDbCommand and OleDbDataAdapter 
                        oleDbCommand.Dispose();
                        oleDbDataAdapter.Dispose();

                        // Close the connection
                        oleDbConnection.Close();

                        return dataTable;
                    }
                }
            }
            return null;
        }

        public DataTable GetDataTable(string appSettingsConnectionStringName, string query)
        {

            using (OleDbConnection oleDbConnection = new OleDbConnection(ConfigurationManager.ConnectionStrings[appSettingsConnectionStringName].ConnectionString))
            {
                // Open the connection
                oleDbConnection.Open();

                //
                //var r = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Indexes,null);

                if (oleDbConnection.State == ConnectionState.Open)
                {
                    DataTable dataTable = GetResutDataTable(query, oleDbConnection);

                    // Close the connection
                    oleDbConnection.Close();

                    return dataTable;
                }
            }
            return null;
        }

        public DataTable GetResutDataTable(string query, OleDbConnection oleDbConnection)
        {
            string schemaQuery = query;

            OleDbCommand oleDbCommand = new OleDbCommand(schemaQuery, oleDbConnection);

            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);

            DataSet schemaDataTable = new DataSet();
            oleDbDataAdapter.FillSchema(schemaDataTable, SchemaType.Source);

            

            // Dispose OleDbCommand and OleDbDataAdapter 
            oleDbCommand.Dispose();
            oleDbDataAdapter.Dispose();

            oleDbCommand = new OleDbCommand(FormatQuery(schemaDataTable.Tables[0].Columns, query), oleDbConnection);

            oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);

            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);

            // Dispose OleDbCommand and OleDbDataAdapter 
            oleDbCommand.Dispose();
            oleDbDataAdapter.Dispose();
            return dataTable;
        }

        #endregion

        #region Private Methods
        private string BuildQuery(DataColumnCollection columnCollection, string tableName)
        {
            string query = "SELECT ";


            foreach (DataColumn column in columnCollection)
            {
                if (column.DataType == typeof(Decimal))
                    query = query + "Cast(" + column.ColumnName + " As NUMERIC(20,2)) " + column.ColumnName + ",";
                else
                    query = query + column.ColumnName + ",";
            }

            query = query.Remove(query.Length - 1);

            query = query + " FROM " + tableName;

            return query;
        }

        private string FormatQuery(DataColumnCollection columnCollection, string query)
        {
            if (query.ToLower().Contains("count("))
                return query;

            var numericColumns = GetNumericColumns();
            string selectedColumn = "";
            foreach (DataColumn column in columnCollection)
            {
                var columnName = column.ColumnName;

                if (column.DataType == typeof(Decimal))
                {
                    if (columnName.GetLast(2) == "_a")
                    {
                        var orgName = columnName.Replace("_a", "");

                        selectedColumn = selectedColumn + "Cast(" + orgName + " As " +
                                         GetNumericColumnPercision(columnName, numericColumns)
                                         + ") " + columnName + ",";
                    }
                    else
                    {
                        selectedColumn = selectedColumn + "Cast(" + columnName + " As " +
                                         GetNumericColumnPercision(columnName, numericColumns)
                                         + ") " + columnName + ",";
                    }
                }
                else
                {
                    if (columnName.GetLast(2) == "_a")
                    {
                        var orgName = columnName.Replace("_a", "");
                        selectedColumn = selectedColumn + orgName + " " + columnName + " ,";
                    }
                    else
                    {
                        selectedColumn = selectedColumn + columnName + ",";
                    }
                }
            }
            if (selectedColumn.Length > 1)
                selectedColumn = selectedColumn.Remove(selectedColumn.Length - 1);


            return query.Replace("*", selectedColumn);
        }

        private SortedDictionary<string, string> GetNumericColumns()
        {
            SortedDictionary<string, string> numericColumns = new SortedDictionary<string, string>();
            foreach (var line in File.ReadAllText("DBF_Decimal_Columns.txt").Split(';'))
            {
                var list = line.Split('#');
                if (list.Count() == 2)
                {
                    var key = list[0].Trim().ToUpper();
                    var value = list[1].Trim();
                    if (numericColumns.ContainsKey(key))
                        numericColumns[key] = value;
                    else
                        numericColumns.Add(key, value);
                }
            }
            return numericColumns;
        }

        private string GetNumericColumnPercision(string columnName, SortedDictionary<string, string> numericColumns)
        {
            columnName = columnName.ToUpper();
            if (numericColumns.ContainsKey(columnName))
            {
                return numericColumns[columnName];
            }
            return "NUMERIC(20,3)";
        }
        #endregion


        public DataTable GetResultDataTable(string query, System.Data.SqlClient.SqlConnection cn)
        {
            throw new NotImplementedException();
        }
    }
}
