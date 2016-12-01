using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public class CodeSeparator
    {
        #region Properties

        public ProjectDetail ProjectDetail { get; set; }

        public DataTable FileDataTable { get; set; }

        public DataTable MethodsDataTable { get; set; }

        public DataTable LinksDataTable { get; set; }

        #endregion

        #region Constructors

        public CodeSeparator()
        {
            FileDataTable = new DataTable();
            FileDataTable.Columns.Add(DataTableHelper.GetAutoIncrementColumn("ID"));
            FileDataTable.Columns.Add("FileName");
            FileDataTable.Columns.Add("FilePath");
            FileDataTable.Columns.Add("FileType");

            MethodsDataTable = new DataTable();
            MethodsDataTable.Columns.Add(DataTableHelper.GetAutoIncrementColumn("ID"));
            MethodsDataTable.Columns.Add("FileID", typeof(Int32));
            MethodsDataTable.Columns.Add("MethodType");
            MethodsDataTable.Columns.Add("Code");
        }
        #endregion

        #region Public Methos

        public void Process(ProjectDetail projectDetail)
        {
            ProjectDetail = projectDetail;

            ProcessLibCode(ProjectDetail.LibCodeDataTable);

            ProcessFormCode(ProjectDetail.FormCodeDataTable);

            ProcessMenuCode(ProjectDetail.MenuCodeDataTable);

            ProcessReportCode(ProjectDetail.ReportCodeDataTable);

            var toDoc = new ToDoc();
            toDoc.Createsqltable(ProjectDetail.FormCodeDataTable, "Forms");

        }
        #endregion

        #region Private Methods

        private void ProcessLibCode(DataTable libCodeDataTable)
        {
            foreach (DataRow dataRow in libCodeDataTable.Rows)
            {
                // PROCEDURES
                FillMethods(dataRow, RegexManager.GetProcedureRegex(), 1, "Lib");
                // FUNCTIONS
                FillMethods(dataRow, RegexManager.GetFunctionRegex(), 2, "Lib");
            }
        }

        private void ProcessFormCode(DataTable formCodeDataTable)
        {
            foreach (DataRow dataRow in formCodeDataTable.Rows)
            {
                // PROCEDURES
                FillMethods(dataRow, RegexManager.GetProcedureRegex(), 1, "Form");
                // FUNCTIONS
                FillMethods(dataRow, RegexManager.GetFunctionRegex(), 2, "Form");
            }
        }

        private void ProcessMenuCode(DataTable menuCodeDataTable)
        {
            foreach (DataRow dataRow in menuCodeDataTable.Rows)
            {
                // PROCEDURES
                FillMethods(dataRow, RegexManager.GetProcedureRegex(), 1, "Menu");
                // FUNCTIONS
                FillMethods(dataRow, RegexManager.GetFunctionRegex(), 2, "Menu");
            }
        }

        private void ProcessReportCode(DataTable reportCodeDataTable)
        {
            foreach (DataRow dataRow in reportCodeDataTable.Rows)
            {
                // PROCEDURES
                FillMethods(dataRow, RegexManager.GetProcedureRegex(), 1, "Report");
                // FUNCTIONS
                FillMethods(dataRow, RegexManager.GetFunctionRegex(), 2, "Report");
            }
        }

        private void FillMethods(DataRow dataRow, Regex regex, int methodType, string fileType)
        {
            string fileName = dataRow[ProjectDetail.ColFileName].ConvertToString();
            string filePath = dataRow[ProjectDetail.ColFilePath].ConvertToString();

            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var columnValue = dataRow[dataColumn.ColumnName].ConvertToString();

                if (columnValue == null)
                    continue;

                var matches = regex.Matches(columnValue);

                foreach (Match match in matches)
                {
                    if (match != null)
                    {
                        var contents = match.Groups[0].Value;
                        AddMethodsDataRow(GetFileId(fileName, filePath, fileType), methodType, contents);
                    }
                }
            }
        }

        private void AddMethodsDataRow(int fileId, int methodType, string code)
        {
            var newMethodsDataRow = MethodsDataTable.NewRow();
            newMethodsDataRow["FileID"] = fileId;
            newMethodsDataRow["MethodType"] = methodType;
            newMethodsDataRow["Code"] = code;

            MethodsDataTable.Rows.Add(newMethodsDataRow);
        }

        private int GetFileId(string fileName, string filePath, string fileType)
        {
            foreach (DataRow dataRow in FileDataTable.Rows)
            {
                if (dataRow["FileName"].ConvertToString() == fileName 
                    && dataRow["FilePath"].ConvertToString() == filePath
                    && dataRow["FileType"].ConvertToString() == fileType)
                {
                    return dataRow["ID"].ConvertToInt32();
                }
            }

            return AddFileDataRow(fileName, filePath, fileType);
        }

        private int AddFileDataRow(string fileName, string filePath, string fileType)
        {
            var fileDataRow = FileDataTable.NewRow();
            fileDataRow["FileName"] = fileName;
            fileDataRow["FilePath"] = filePath;
            fileDataRow["FileType"] = fileType;

            FileDataTable.Rows.Add(fileDataRow);

            return fileDataRow["ID"].ConvertToInt32();
        }
        #endregion
    }
}
