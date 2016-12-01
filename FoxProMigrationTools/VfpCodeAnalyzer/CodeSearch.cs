using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace VfpCodeAnalyzer
{
    public class CodeSearch
    {
        #region Properties

        public CodeSearchOptions Options { get; private set; }

        public DataTable ResultsDataTable { get; private set; }
        #endregion

        #region Constructor

        public CodeSearch()
        {
            ResultsDataTable = new DataTable();
            ResultsDataTable.Columns.Add("FileName");
            ResultsDataTable.Columns.Add("FilePath");
            ResultsDataTable.Columns.Add("Property");
            ResultsDataTable.Columns.Add("Contents");
            ResultsDataTable.Columns.Add("Class");
            ResultsDataTable.Columns.Add("ClassLoc");
            ResultsDataTable.Columns.Add("BaseClass");
            ResultsDataTable.Columns.Add("ObjName");
            ResultsDataTable.Columns.Add("Parent");
        }
        #endregion

        #region Public Methods

        public void Search(ProjectDetail projectDetail, CodeSearchOptions options)
        {
            Options = options;

            SearchDataTable(projectDetail.FormCodeDataTable);
            SearchDataTable(projectDetail.LibCodeDataTable);
            SearchDataTable(projectDetail.MenuCodeDataTable);
            SearchDataTable(projectDetail.ReportCodeDataTable);
        }
        #endregion

        #region Private Methods

        private void SearchDataTable(DataTable dataTable)
        {
            if (dataTable == null)
                return;

            bool isAtleastOneFileProcessed = false;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string fileName = dataRow[ProjectDetail.ColFileName].ConvertToString();
                string filePath = dataRow[ProjectDetail.ColFilePath].ConvertToString();

                if (Options.SearchInSelectedFiles)
                {
                    bool isFileFound = Options.FileDetails.FirstOrDefault(file => file.IsIncluded && file.FileName == fileName) != null;
                    if (!isFileFound)
                        continue;

                    isAtleastOneFileProcessed = true;
                }

                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    var columnValue = dataRow[dataColumn.ColumnName].ConvertToString();

                    if (columnValue == null)
                        continue;

                    if (IsSearchTextPresent(columnValue))
                    {
                        List<string> matchedContentList;
                        if (IsRegexMatchPresent(columnValue, RegexManager.GetProcedureRegex(), out matchedContentList))
                        {
                            foreach (string matchedContent in matchedContentList)
                            {
                                AddToResultsDataTable(dataColumn.ColumnName, matchedContent, fileName, filePath, dataRow);
                            }
                            //continue;
                        }
                        if (IsRegexMatchPresent(columnValue, RegexManager.GetFunctionRegex(), out matchedContentList))
                        {
                            foreach (string matchedContent in matchedContentList)
                            {
                                AddToResultsDataTable(dataColumn.ColumnName, matchedContent, fileName, filePath, dataRow);
                            }
                            //continue;
                        }
                        AddToResultsDataTable(dataColumn.ColumnName, columnValue, fileName, filePath, dataRow);
                    }
                }
            }

            if (Options.SearchInSelectedFiles && isAtleastOneFileProcessed == false)
            {
                var list = Options.FileDetails.Where(file => file.IsIncluded);
                if (list.Count() == 0)
                    System.Diagnostics.Debugger.Break();
            }
        }

        private bool IsRegexMatchPresent(string columnValue, Regex regex, out List<string> contentList)
        {
            var matches = regex.Matches(columnValue);

            contentList = new List<string>();

            foreach (Match match in matches)
            {
                if (match != null)
                {
                    var content = match.Groups[0].Value;
                    if (IsSearchTextPresent(content))
                        contentList.Add(content);
                }
            }
            return contentList.Count > 0;
        }

        private void AddToResultsDataTable(string columnName, string columnValue, string fileName, string filePath, DataRow data)
        {
            var newResultDataRow = ResultsDataTable.NewRow();
            newResultDataRow["FileName"] = fileName;
            newResultDataRow["FilePath"] = filePath;
            newResultDataRow["Property"] = columnName;
            newResultDataRow["Contents"] = columnValue.Trim();

            if (data != null)
            {
                newResultDataRow["Class"] = GetColumnValue(data, "Class");
                newResultDataRow["ClassLoc"] = GetColumnValue(data, "ClassLoc");
                newResultDataRow["BaseClass"] = GetColumnValue(data, "BaseClass");
                newResultDataRow["ObjName"] = GetColumnValue(data, "ObjName");
                newResultDataRow["Parent"] = GetColumnValue(data, "Parent");
            }

            if (!IsAlreadyPresent(columnName, columnValue, fileName, filePath))
                ResultsDataTable.Rows.Add(newResultDataRow);
        }

        private bool IsAlreadyPresent(string columnName, string columnValue, string fileName, string filePath)
        {
            foreach (DataRow row in ResultsDataTable.Rows)
            {
                if (row["FileName"].ConvertToString() == fileName && row["FilePath"].ConvertToString() == filePath 
                    && row["Property"].ConvertToString() == columnName && row["Contents"].ConvertToString() == columnValue.Trim())
                    return true;
            }
            return false;
        }

        private string GetColumnValue(DataRow dataRow, string columnName)
        {
            bool isValidColumn = false;
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                if (columnName.ToLower() == column.ColumnName.ToLower())
                {
                    isValidColumn = true;
                    break;
                }
            }
            if (isValidColumn == false)
                return "N/A";

            return dataRow[columnName].ConvertToString();
        }

        private bool IsSearchTextPresent(string contents)
        {
            if (string.IsNullOrEmpty(Options.SearchString))
                return false;

            if (Options.IsCaseSensitive)
                return contents.Contains(Options.SearchString);

            return contents.ToLower().Contains(Options.SearchString.ToLower());
        }
        #endregion
    }
}
