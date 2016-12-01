using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataComparer.Common;
using DataComparer.Common.Contracts;
using DataComparer.Common.Domain;

namespace DataComparer.Dal
{
    public class SqlDataProvider : DataProviderBase, IDataProvider
    {

        #region IDataProvider Implementation
        public DataTable GetDataTable(string appSettingsConnectionStringName, TableDetail tableDetail)
        {
            if (!IsArgumentsValid(appSettingsConnectionStringName, tableDetail))
                return null;

            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings[appSettingsConnectionStringName].ConnectionString))
            {
                cn.Open();

                string query = "SELECT * FROM " + tableDetail.TableName
                    + GetWhereClause(tableDetail.WhereClauseConditions, Constants.AppConfigFirstDatabaseConnectionStringName == appSettingsConnectionStringName);

                using (SqlCommand command = new SqlCommand(query, cn))
                {
                    DataTable dataTable = new DataTable();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlDataAdapter.SelectCommand = command;
                    sqlDataAdapter.Fill(dataTable);

                    cn.Close();

                    return dataTable;
                }
            }
        }

        public DataTable GetDataTable(string appSettingsConnectionStringName, string query)
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings[appSettingsConnectionStringName].ConnectionString))
            {
                cn.Open();
                
                var dataTable = GetResultDataTable(query, cn);
                cn.Close();

                return dataTable;
            }
        }

        public DataTable GetResultDataTable(string query, SqlConnection cn)
        {
            using (SqlCommand command = new SqlCommand(query, cn))
            {
                DataTable dataTable = new DataTable();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                sqlDataAdapter.SelectCommand.CommandTimeout = 0;
                sqlDataAdapter.Fill(dataTable);

                return dataTable;
            }
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion






        public DataTable GetResutDataTable(string query, System.Data.OleDb.OleDbConnection oleDbConnection)
        {
            throw new NotImplementedException();
        }
    }
}
