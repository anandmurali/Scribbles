using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public class ProjectDetail
    {
        #region Constants

        public const string ColFileName = "CUSFileName";

        public const string ColFilePath = "CUSFilePath";

        public const string ColContents = "CUSContents";
        #endregion

        #region Properties

        public string ProjectName { get; set; }

        public string ProjectPath { get; set; }

        public string ProjectDirectory { get; set; }

        public DataTable FileNamesDataTable { get; set; }

        public DataTable FormCodeDataTable { get; set; }

        public DataTable LibCodeDataTable { get; set; }

        public DataTable MenuCodeDataTable { get; set; }

        public DataTable ReportCodeDataTable { get; set; }

        public List<ErrorDetail> ErrorList { get; set; }

        public int Errors { get; set; }

        public int ReportErrors { get; set; }
        #endregion

        #region Constructor

        public ProjectDetail()
        {
            FileNamesDataTable = new DataTable();

            LibCodeDataTable = new DataTable();
            LibCodeDataTable.Columns.Add(ColFileName);
            LibCodeDataTable.Columns.Add(ColFilePath);
            LibCodeDataTable.Columns.Add(ColContents);

            ErrorList = new List<ErrorDetail>();
        }
        #endregion

        #region Public Methods

        public void MergeFormCodeDataTable(DataTable dataTable, string filePath)
        {
            if (FormCodeDataTable == null)
            {
                FormCodeDataTable = dataTable.Clone();
                AddCustomColumns(FormCodeDataTable);
            }

            // Check if all columns are same
            foreach (DataColumn column in dataTable.Columns)
            {
                bool isFound = false;
                foreach (DataColumn existingColumn in FormCodeDataTable.Columns)
                {
                    if (column.ColumnName == existingColumn.ColumnName)
                    {
                        isFound = true;
                        break;
                    }
                }
                if (isFound == false)
                    System.Diagnostics.Debugger.Break();
            }

            string fileName = Path.GetFileName(filePath);

            dataTable.Columns.Add(ColFileName);
            dataTable.Columns.Add(ColFilePath);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                dataRow[ColFileName] = fileName;
                dataRow[ColFilePath] = filePath;
            }


            FormCodeDataTable.Merge(dataTable);
        }

        public void MergeMenuCodeDataTable(DataTable dataTable)
        {
            if (MenuCodeDataTable == null)
            {
                MenuCodeDataTable = dataTable.Clone();
                AddCustomColumns(MenuCodeDataTable);
            }

            MenuCodeDataTable.Merge(dataTable);
        }

        public void MergeReportCodeDataTable(DataTable dataTable)
        {
            if (ReportCodeDataTable == null)
            {
                ReportCodeDataTable = dataTable.Clone();
                AddCustomColumns(ReportCodeDataTable);
            }

            ReportCodeDataTable.Merge(dataTable);
        }

        public void AddReportError(string reportPath, Exception exception)
        {
            ReportErrors++;

            var errorDetail = new ErrorDetail();
            errorDetail.Error = exception.ToLogString();
            errorDetail.FullPath = reportPath;
            errorDetail.FileName = Path.GetFileName(reportPath);

            ErrorList.Add(errorDetail);
        }
        #endregion

        #region Private Methods

        private void AddCustomColumns(DataTable dataTable)
        {
            dataTable.Columns.Add(ColFileName);
            dataTable.Columns.Add(ColFilePath);
            dataTable.Columns.Add(ColContents);
        }
        #endregion
    }
}
