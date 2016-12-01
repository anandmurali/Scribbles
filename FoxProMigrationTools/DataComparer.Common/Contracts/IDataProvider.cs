using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataComparer.Common.Domain;

namespace DataComparer.Common.Contracts
{
    public interface IDataProvider
    {
        DataTable GetDataTable(string appSettingsConnectionStringName, TableDetail tableDetail);

        DataTable GetDataTable(string appSettingsConnectionStringName,string query);

        DataTable GetResutDataTable(string query, OleDbConnection oleDbConnection);

        DataTable GetResultDataTable(string query, SqlConnection cn);
    }
}
