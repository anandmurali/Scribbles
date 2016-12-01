using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using DataTable = System.Data.DataTable;

namespace VfpCodeAnalyzer
{
    public class ToDoc
    {
        public void Save(DataTable dataTable, string filePath = null)
        {
            Microsoft.Office.Interop.Word.Application winword = new Microsoft.Office.Interop.Word.Application();

            //Set animation status for word application
            winword.ShowAnimation = false;

            //Set status for word application is to be visible or not.
            winword.Visible = false;

            //Create a missing variable for missing value
            object missing = System.Reflection.Missing.Value;

            //Create a new document
            Microsoft.Office.Interop.Word.Document document = winword.Documents.Add();

            var para1 = document.Paragraphs.Add();

            para1.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            para1.Range.Text = "Caption";
            

            object start = 0;
            object end = 0;

            Range rng = document.Range(ref start, ref end);
            rng.Text = "New Text";

            var tocRange = document.Range(0, 0);
            var toc = document.TablesOfContents.Add(
                Range: tocRange,
                UseHeadingStyles: true);
            toc.Update();

            var tocTitleRange = document.Range(0, 0);
            tocTitleRange.Text = "List of Images";
            tocTitleRange.InsertParagraphAfter();
            tocTitleRange.set_Style("Title");

            document.SaveAs("E:\\First.docx");
            document.Close();

            winword.Quit();
        }



        public void Createsqltable(DataTable dt, string tablename)
        {
            string strconnection = ConfigurationManager.ConnectionStrings["DB_CONNECTION"].ConnectionString;
            string table = "";
            table += "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + tablename + "]') AND type in (N'U'))";
            table += "BEGIN ";
            table += "create table " + tablename + "";
            table += "(";
            dt.Columns.Remove("user");
            dt.Columns.Remove("objcode");
            for (int i = 0; i < dt.Columns.Count; i++)
            {

                if (i != dt.Columns.Count - 1)
                    table += dt.Columns[i].ColumnName + " " + "varchar(max)" + ",";
                else
                    table += dt.Columns[i].ColumnName + " " + "varchar(max)";
            }
            table += ") ";
            table += "END";
            InsertQuery(table, strconnection);
            CopyData(strconnection, dt, tablename);
        }
        public void InsertQuery(string qry, string connection)
        {


            SqlConnection _connection = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = qry;
            cmd.Connection = _connection;
            _connection.Open();
            cmd.ExecuteNonQuery();
            _connection.Close();
        }
        public static void CopyData(string connStr, DataTable dt, string tablename)
        {
            using (SqlBulkCopy bulkCopy =
            new SqlBulkCopy(connStr, SqlBulkCopyOptions.TableLock))
            {
                bulkCopy.DestinationTableName = tablename;
                bulkCopy.WriteToServer(dt);
            }
        }
    }
}
