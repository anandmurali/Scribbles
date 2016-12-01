using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace VfpCodeAnalyzer
{
    public class VfpCodeAnalyzerLogic
    {
        private DataTable displayTable = new DataTable();
        private DataTable dt = new DataTable();
        private DataRow dr;
        private DataTable vfpProject = new DataTable();
        private string pjxPath;
        private string pjxName;
        private string pjxPathOnly;

        private float pjxLines = 0;
        private float totalLines = 0, numberOfMethods = 0;
        private float codeLines = 0, commentLines = 0, blankLines = 0;
        private float codePercent = 0, commentPercent = 0, blankPercent = 0;
        private float numberOfErrors = 0;
        private bool fileError = false;

        private float fileTotalLines = 0, fileNumberOfMethods = 0;
        private float fileCodeLines = 0, fileCommentLines = 0, fileBlankLines = 0;
        private float fileCodePercent = 0, fileCommentPercent = 0, fileBlankPercent = 0;
        private string ColParent = "Parent";
        private string ColObjName = "ObjName";
        private string ColBaseClass = "BaseClass";
        private string ColClass = "Class";
        private string ColMethodContent = "MethodContent";
        private string ColFile = "File";
        private string ColProperties = "Properties";
        private static readonly string ColExtractedContent = "ExtractedContent";

        #region Properties

        public DataTable ResultsDataTable { get; set; }

        public DataTable VfpDataTable { get; set; }

        public String ProjectPath { get; set; }

        public String SearchString { get; set; }

        public String ProjectName { get; set; }

        public String ProjectDirectory { get; set; }
        #endregion

        #region Constructor

        public VfpCodeAnalyzerLogic()
        {
        }
        #endregion

        #region Public Methods

        public void Process(string path)
        {
            btnProcess_Click(path);
        }
        #endregion


        #region Private Methods

        //private void OpenProject(string theProject)
        //{
        //    ProjectPath = Path.GetFullPath(theProject);
        //    ProjectName = Path.GetFileName(theProject);
        //    ProjectDirectory = Path.GetDirectoryName(theProject);

        //    OleDbConnectionStringBuilder cb = new OleDbConnectionStringBuilder();
        //    cb.Provider = "VFPOLEDB";
        //    cb.DataSource = ProjectPath;
        //    using (OleDbConnection conn = new OleDbConnection(cb.ConnectionString))
        //    {
        //        conn.Open();
        //        if (VfpDataTable == null)
        //            VfpDataTable = new DataTable();

        //        VfpDataTable.Clear();
        //        // 12/30/2011 - Add 'T' to select statement to pull Include records
        //        OleDbDataAdapter da = new OleDbDataAdapter(
        //          "select name, type from '" + ProjectName +
        //          "' where inlist(type, 'P', 'M', 'V', 'T', 'K', 'R') order by type", conn);
        //        try
        //        {
        //            da.Fill(VfpDataTable);


        //        }
        //        catch (OleDbException ode)
        //        {
        //            MessageBox.Show("Error opening Project:\n\n" +
        //                            ode.Message);
        //        }
        //    }
        //}

        private void btnProcess_Click(string path)
        {
            OpenProject(path);
            BuildDisplayTable();
            ReadProject();

            foreach (DataRow row in displayTable.Rows)
            {
                totalLines += Convert.ToInt32(row["totalLines"]);
                numberOfMethods += Convert.ToInt32(row["numberMethods"]);
                blankLines += Convert.ToInt32(row["blankLines"]);
                commentLines += Convert.ToInt32(row["commentLines"]);
                codeLines += Convert.ToInt32(row["codeLines"]);
            }
            //txtNumberOfFiles.Text = pjxLines.ToString();
            //txtTotalLines.Text = totalLines.ToString();
            //txtNumMethods.Text = numberOfMethods.ToString();
            //txtBlankLines.Text = blankLines.ToString();
            //txtCommentLines.Text = commentLines.ToString();
            //txtCodeLines.Text = codeLines.ToString();
            // Calculate the percentages
            codePercent = (codeLines / totalLines) * 100;
            commentPercent = (commentLines / totalLines) * 100;
            blankPercent = (blankLines / totalLines) * 100;

            //txtCodePercent.Text = codePercent.ToString();
            //txtCommentPercent.Text = commentPercent.ToString();
            //txtBlankPercent.Text = blankPercent.ToString();
            //Refresh();
            //lblCurrentFile.Text = "";
            MessageBox.Show("Complete!");
            //btnPrintPreview.Enabled = true;
        }
        /// <summary>
        /// Read the open project records from the vfpProject DataTable, call appropriate analyze routines
        /// to perform the analysis.
        /// </summary>
        public void ReadProject()
        {
            //pbProcess.Maximum = Convert.ToInt32(pjxLines);
            foreach (DataRow r in vfpProject.Rows)
            {
                //pbProcess.Increment(1);
                //lblCurrentFile.Text = r["name"].ToString();

                //if (r["name"].ToString().ToLower().Contains("forex"))
                //{
                //    System.Diagnostics.Debugger.Break();
                //}

                switch (r["type"].ToString())
                {
                    case "P":
                        ProcessProgram(r["name"].ToString());
                        break;
                    case "M":
                        ProcessMenu(r["name"].ToString());
                        break;
                    case "V":
                        ProcessClassLibrary(r["name"].ToString());
                        break;
                    case "K":
                        ProcessForm(r["name"].ToString());
                        break;
                    case "T":
                        ProcessInclude(r["name"].ToString());
                        break;
                    case "R":
                        ProcessReport(r["name"].ToString());
                        break;
                }
            }
        }
        /// <summary>
        /// Analyze .PRG program file.
        /// </summary>
        /// <param name="p">Path and file name of .PRG file to analyze</param>
        private void ProcessProgram(string p)
        {
            string prgPath = pjxPathOnly + "\\" + p;
            string prgName = Path.GetFileName(p);
            try
            {
                ResetFileVariables();

                string contents = File.ReadAllText(prgPath);
                if (string.IsNullOrEmpty(contents) == false)
                {
                    ProcessCodeFromFile(p, contents.ToUpper(), GetProcedureRegex());

                    //if (contents.ToLower().Contains(SearchString.ToLower()))
                    //    System.Diagnostics.Debugger.Break();

                    ProcessCodeFromFile(p, contents.ToUpper(), GetFunctionRegex());
                }

                string[] prgLines = File.ReadAllLines(prgPath);

                //if (p.ToLower().Contains("ftp"))
                //    System.Diagnostics.Debugger.Break();

                fileTotalLines = prgLines.Length;
                foreach (string line in prgLines)
                {
                    //string vLine = line.Replace("\r\n", "\r");
                    //vLine = line.Replace("\t", "");
                    //if (vLine.Length == 0)
                    //    fileBlankLines++;
                    //else
                    //{
                    //    fileNumberOfMethods += Occurs("PROCEDURE ", vLine.ToUpper());
                    //    fileNumberOfMethods += Occurs("FUNCTION ", vLine.ToUpper());

                    //    // Check for lines beginning with && as comments (bad form but VFP accepts it)
                    //    // Correctly account for short lines
                    //    if (vLine.Substring(0, 1) == "*" ||
                    //       (vLine.Length > 2 && vLine.Substring(0, 3) == "*!*") ||
                    //       (vLine.Length > 1 && vLine.Substring(0, 2) == "&&"))
                    //        fileCommentLines++;
                    //    else
                    //        fileCodeLines++;
                    //}
                }
                AddRecordToDisplay(prgName, "Program");
            }
            catch (Exception e)
            {
                AddRecordToDisplay(prgName, "Program");
            }
        }
        /// <summary>
        /// Analyze a Menu (.MNX) file
        /// </summary>
        /// <param name="p">Path and file name of .MNX file to analyze</param>
        private void ProcessMenu(string p)
        {
            string mnxPath = pjxPathOnly + "\\" + p;
            string mnxName = Path.GetFileName(p);

            ResetFileVariables();
            OleDbConnectionStringBuilder cb = new OleDbConnectionStringBuilder();
            cb.Provider = "VFPOLEDB";
            cb.DataSource = mnxPath;
            using (OleDbConnection conn = new OleDbConnection(cb.ConnectionString))
            {
                try
                {
                    conn.Open();
                    dt.Clear();
                    dt.Columns.Clear();
                    OleDbDataAdapter da = new OleDbDataAdapter("select * from '" + mnxName + "' where objtype = 3", conn);
                    try
                    {
                        da.Fill(dt);
                        CreateResultDataTableStructrue(dt);
                    }
                    catch (OleDbException)
                    {
                        numberOfErrors++;
                        fileError = true;
                    }
                    foreach (DataRow r in dt.Rows)
                    {
                        string m;
                        m = r["procedure"].ToString();
                        if (m.Length > 0)
                        {
                            fileNumberOfMethods += Occurs("PROCEDURE", m);
                            ProcessCodeFromFile(p, m, GetProcedureRegex());
                        }
                        CheckInColumns(p, r, m);
                    }
                    fileTotalLines = fileBlankLines + fileCommentLines + fileCodeLines;
                }
                catch
                {
                }
            }
            AddRecordToDisplay(mnxName, "Menu");
        }

        private void CreateResultDataTableStructrue(DataTable dataTable)
        {
            if (ResultsDataTable == null)
            {
                ResultsDataTable = dataTable.Clone();
                ResultsDataTable.Columns.Add(ColExtractedContent);
            }
        }

        /// <summary>
        /// Analyze Class Library .VCX file
        /// </summary>
        /// <param name="p">Path and file name of .VCX file to analyze</param>
        private void ProcessClassLibrary(string p)
        {
            string vcxPath = pjxPathOnly + "\\" + p;
            string vcxName = Path.GetFileName(p);

            ResetFileVariables();
            OleDbConnectionStringBuilder cb = new OleDbConnectionStringBuilder();
            cb.Provider = "VFPOLEDB";
            cb.DataSource = vcxPath;
            using (OleDbConnection conn = new OleDbConnection(cb.ConnectionString))
            {
                try
                {
                    conn.Open();
                    dt.Clear();
                    dt.Columns.Clear();
                    OleDbDataAdapter da = new OleDbDataAdapter("select * from '" + vcxName + "'", conn);
                    try
                    {
                        da.Fill(dt);
                        CreateResultDataTableStructrue(dt);
                    }
                    catch (OleDbException)
                    {
                        numberOfErrors++;
                        fileError = true;
                    }
                    foreach (DataRow r in dt.Rows)
                    {
                        string m;
                        m = r["methods"].ToString();
                        if (m.Length > 0)
                        {
                            ProcessCodeFromFile(p, m, GetProcedureRegex());
                        }

                        CheckInColumns(p, r, m);
                    }
                    fileTotalLines = fileBlankLines + fileCommentLines + fileCodeLines;
                }
                catch
                {
                }
            }
            AddRecordToDisplay(vcxName, "Class Library");
        }

        private void CopyDataRow(DataRow toDataRow, DataRow fromDataRow)
        {
            toDataRow[ColClass] = fromDataRow[ColClass];
            toDataRow[ColBaseClass] = fromDataRow[ColBaseClass];
            toDataRow[ColObjName] = fromDataRow[ColObjName];
            toDataRow[ColParent] = fromDataRow[ColParent];
            toDataRow[ColProperties] = fromDataRow[ColProperties];
        }

        /// <summary>
        /// Analyze a Form (.SCX) file
        /// </summary>
        /// <param name="p">Path and file name of .SCX file to analyze</param>
        private void ProcessForm(string p)
        {
            string scxPath = pjxPathOnly + "\\" + p;
            string scxName = Path.GetFileName(p);

            ResetFileVariables();
            OleDbConnectionStringBuilder cb = new OleDbConnectionStringBuilder();
            cb.Provider = "VFPOLEDB";
            cb.DataSource = scxPath;
            using (OleDbConnection conn = new OleDbConnection(cb.ConnectionString))
            {
                try
                {


                    conn.Open();
                    dt.Clear();
                    dt.Columns.Clear();
                    OleDbDataAdapter da = new OleDbDataAdapter("select * from '" + scxName + "'", conn);
                    try
                    {
                        da.Fill(dt);
                        CreateResultDataTableStructrue(dt);
                        if (p.Contains("evaluator"))
                        {
                            //System.Diagnostics.Debugger.Break();
                            //new ToDoc().Save(dt);
                        }
                    }
                    catch (OleDbException)
                    {
                        numberOfErrors++;
                        fileError = true;
                    }
                    foreach (DataRow r in dt.Rows)
                    {
                        string m;
                        m = r["methods"].ToString();

                        if (m.Length > 0)
                        {
                            fileNumberOfMethods += Occurs("PROCEDURE", m);
                            ProcessCodeFromFile(p, m, GetProcedureRegex());
                        }

                        CheckInColumns(p, r, m);
                    }
                    fileTotalLines = fileBlankLines + fileCommentLines + fileCodeLines;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            AddRecordToDisplay(scxName, "Form");
        }

        private void CheckInColumns(string p, DataRow r, string m)
        {
            //CheckColumn(p, r, m, ColBaseClass);
            //CheckColumn(p, r, m, ColClass);
            //CheckColumn(p, r, m, ColObjName);
            //CheckColumn(p, r, m, ColParent);;
            //CheckColumn(p, r, m, ColProperties);
            foreach (DataColumn column in r.Table.Columns)
            {
                CheckColumn(p,r,m, column.ColumnName);
            }
        }

        private void CheckColumn(string p, DataRow r, string m, string columnName)
        {
            if (columnName.ToLower() == "methods")
                return;

            if (r[columnName].ToString().ToLower().Contains(SearchString.ToLower()))
            {
                AddToResultDataTable(r);
            }
        }

        private void AddToResultDataTable(DataRow dataRow, string extractedContent = null)
        {
            if (ResultsDataTable == null)
            {
                ResultsDataTable = dataRow.Table.Clone();
                ResultsDataTable.Columns.Add(ColExtractedContent);
            }

            var newDataRow = ResultsDataTable.NewRow();
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                newDataRow[column.ColumnName] = dataRow[column.ColumnName];
            }

            if (extractedContent != null)
            {
                newDataRow[ColExtractedContent] = extractedContent;
            }

            ResultsDataTable.Rows.Add(newDataRow);
        }

        private Regex GetProcedureRegex()
        {
            return new Regex(Regex.Escape("PROCEDURE") + @"([\s\S]*?)" + Regex.Escape("ENDPROC"));
            //return new Regex(@"PROCEDURE(?:((?!PROCEDURE)|ENDPROC|(?!FUNCTION))[\s\S])*");
        }

        private Regex GetFunctionRegex()
        {

            return new Regex(@"FUNCTION(?:((?!FUNCTION)|ENDFUNC|(?!PROCEDURE))[\s\S])*");
        }

        private void ProcessCodeFromFile(string fileName, string codeString, Regex regex)
        {
            var matches = regex.Matches(codeString);
            foreach (Match match in matches)
            {
                if (match != null)
                {
                    var contents = match.Groups[0].Value;

                    if (string.IsNullOrEmpty(SearchString) == false)
                    {
                        if (contents.ToLower().Contains(SearchString.ToLower()))
                        {
                            var row = ResultsDataTable.NewRow();
                            row[0] = fileName;
                            row[ColExtractedContent] = contents;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Analyze a .H include file
        /// </summary>
        /// <param name="p">Path and file name of the .H file to analyze</param>
        private void ProcessInclude(string p)
        {
            string hPath = pjxPathOnly + "\\" + p;
            string hName = Path.GetFileName(p);
            try
            {
                ResetFileVariables();

                //string[] hLines = File.ReadAllLines(hPath);

                ProcessCodeFromFile(p, File.ReadAllText(hPath).ToUpper(), GetProcedureRegex());
                ProcessCodeFromFile(p, File.ReadAllText(hPath).ToUpper(), GetFunctionRegex());

                //fileTotalLines = hLines.Length;
                //foreach (string vLine in hLines)
                //{
                //    if (vLine.Length == 0)
                //        fileBlankLines++;
                //    else
                //    {
                //        fileNumberOfMethods += Occurs("PROCEDURE ", vLine.ToUpper());
                //        ProcessMethods(p, vLine.ToUpper(), GetProcedureRegex(), null);
                //        fileNumberOfMethods += Occurs("FUNCTION ", vLine.ToUpper());
                //        // TODO
                //    }
                //}
                AddRecordToDisplay(hName, "Include");
            }
            catch
            {
                AddRecordToDisplay(hName, "Include");
            }
        }
        /// <summary>
        /// Analyze .FRX report file
        /// </summary>
        /// <param name="p">.FRX file to analyze</param>
        private void ProcessReport(string p)
        {
            string frxPath = pjxPathOnly + "\\" + p;
            string frxName = Path.GetFileName(p);

            ResetFileVariables();
            OleDbConnectionStringBuilder cb = new OleDbConnectionStringBuilder();
            cb.Provider = "VFPOLEDB";
            cb.DataSource = frxPath;
            using (OleDbConnection conn = new OleDbConnection(cb.ConnectionString))
            {
                try
                {
                    conn.Open();
                    dt.Clear();
                    dt.Columns.Clear();
                    OleDbDataAdapter da = new OleDbDataAdapter("select * from '" + frxName + "' where objtype = 8", conn);
                    try
                    {
                        da.Fill(dt);
                        CreateResultDataTableStructrue(dt);
                    }
                    catch (OleDbException)
                    {
                        numberOfErrors++;
                        fileError = true;
                    }
                    foreach (DataRow r in dt.Rows)
                    {
                        string m;
                        m = r["expr"].ToString();
                        if (m.Length > 0)
                        {
                            fileNumberOfMethods += Occurs("PROCEDURE", m);
                            ProcessCodeFromFile(p, m, GetProcedureRegex());
                        }
                        // Added for VFP 9 reports 8/31/2011
                        m = r["supexpr"].ToString();
                        if (m.Length > 0)
                        {
                            fileNumberOfMethods += Occurs("PROCEDURE", m);
                            ProcessCodeFromFile(p, m, GetProcedureRegex());
                        }

                        CheckInColumns(p, r, m);
                    }
                    fileTotalLines = fileBlankLines + fileCommentLines + fileCodeLines;
                }
                catch
                {
                }
            }
            AddRecordToDisplay(frxName, "Report");
        }
        /// <summary>
        /// Open a VFP .PJX Project file, load a DataTable with the PJX contents
        /// </summary>
        /// <param name="theProject">Path and file name of the .PJX file to open</param>
        private void OpenProject(string theProject)
        {
            pjxPath = Path.GetFullPath(theProject);
            pjxName = Path.GetFileName(theProject);
            pjxPathOnly = Path.GetDirectoryName(theProject);

            OleDbConnectionStringBuilder cb = new OleDbConnectionStringBuilder();
            cb.Provider = "VFPOLEDB";
            cb.DataSource = pjxPath;
            using (OleDbConnection conn = new OleDbConnection(cb.ConnectionString))
            {
                conn.Open();
                vfpProject.Clear();
                vfpProject.Clear();
                // 12/30/2011 - Add 'T' to select statement to pull Include records
                OleDbDataAdapter da = new OleDbDataAdapter(
                  "select name, type from '" + pjxName +
                  "' where inlist(type, 'P', 'M', 'V', 'T', 'K', 'R') order by type", conn);
                try
                {
                    da.Fill(vfpProject);
                    pjxLines = vfpProject.Rows.Count;
                }
                catch (OleDbException ode)
                {
                    MessageBox.Show("Error opening Project:\n\n" +
                                    ode.Message);
                }
            }
        }
        /// <summary>
        /// Add a record to the displayResults DataTable and refresh the grid
        /// </summary>
        /// <param name="fileName">Name of file that was analyzed</param>
        /// <param name="fileType">Type of the file, ie, report, form, menu, etc.</param>
        private void AddRecordToDisplay(string fileName, string fileType)
        {
            CalculateFilePercentages();
            dr = displayTable.NewRow();
            dr["fileName"] = fileName;
            dr["fileType"] = fileType;
            if (fileError)
                dr["fileName"] += " error";
            dr["totalLines"] = fileTotalLines.ToString().PadLeft(5);
            dr["numberMethods"] = fileNumberOfMethods.ToString().PadLeft(5);
            dr["codeLines"] = fileCodeLines.ToString().PadLeft(5);
            dr["commentLines"] = fileCommentLines.ToString().PadLeft(5);
            dr["blankLines"] = fileBlankLines.ToString().PadLeft(5);
            dr["codePercent"] = fileCodePercent.ToString("N").PadLeft(10);
            dr["commentPercent"] = fileCommentPercent.ToString("N").PadLeft(10);
            dr["blankPercent"] = fileBlankPercent.ToString("N").PadLeft(10);
            displayTable.Rows.Add(dr);
            //dgvResults.DataSource = displayTable;
            //Refresh();
        }
        /// <summary>
        /// Calculates the percentages for the current file
        /// </summary>
        private void CalculateFilePercentages()
        {
            if (fileTotalLines > 0)
            {
                fileCodePercent = (fileCodeLines / fileTotalLines) * 100;
                fileCommentPercent = (fileCommentLines / fileTotalLines) * 100;
                fileBlankPercent = (fileBlankLines / fileTotalLines) * 100;
            }
        }
        /// <summary>
        /// Resets File Variables to zero
        /// </summary>
        private void ResetFileVariables()
        {
            fileTotalLines = 0;
            fileNumberOfMethods = 0;
            fileCodeLines = 0;
            fileCommentLines = 0;
            fileBlankLines = 0;
            fileBlankPercent = 0;
            fileCodePercent = 0;
            fileCommentPercent = 0;
        }
        /// <summary>
        /// Define the schema of the DataTable tied to the display grid.
        /// </summary>
        private void BuildDisplayTable()
        {
            displayTable.Clear();
            displayTable.Columns.Clear();

            displayTable.Columns.Add(new DataColumn("fileName", typeof(string)));
            displayTable.Columns.Add(new DataColumn("fileType", typeof(string)));

            displayTable.Columns.Add(new DataColumn("totalLines", typeof(string)));
            displayTable.Columns.Add(new DataColumn("numberMethods", typeof(string)));

            displayTable.Columns.Add(new DataColumn("codeLines", typeof(string)));
            displayTable.Columns.Add(new DataColumn("commentLines", typeof(string)));
            displayTable.Columns.Add(new DataColumn("blankLines", typeof(string)));

            displayTable.Columns.Add(new DataColumn("codePercent", typeof(string)));
            displayTable.Columns.Add(new DataColumn("commentPercent", typeof(string)));
            displayTable.Columns.Add(new DataColumn("blankPercent", typeof(string)));

            // Set the captions for the columns in the table
            //dgvResults.Columns[0].HeaderText = "File Name";
            //dgvResults.Columns[1].HeaderText = "File Type";

            //dgvResults.Columns[2].HeaderText = "# Lines";
            //dgvResults.Columns[3].HeaderText = "# Methods";

            //dgvResults.Columns[4].HeaderText = "# Lines Code";
            //dgvResults.Columns[5].HeaderText = "# Comment Lines";
            //dgvResults.Columns[6].HeaderText = "# Blank Lines";

            //dgvResults.Columns[7].HeaderText = "% Code";
            //dgvResults.Columns[8].HeaderText = "% Comments";
            //dgvResults.Columns[9].HeaderText = "% Blank Lines";
        }
        /// <summary>
        /// Counts number of occurrances of <code>cExpression</code> in <code>cString</code>
        /// </summary>
        /// <param name="cString">String to search within</param>
        /// <param name="cExpression">String to look for</param>
        /// <returns></returns>
        public static int Occurs(string cString, string cExpression)
        {
            int nPos = 0;
            int nOccured = 0;
            do
            {
                //Look for the search string in the expression
                nPos = cExpression.IndexOf(cString, nPos);
                if (nPos < 0)
                {
                    //This means that we did not find the item			
                    break;
                }
                else
                {
                    //Increment the occured counter based on the current mode we are in			
                    nOccured++;
                    nPos++;
                }
            } while (true);
            //Return the number of occurences	
            return nOccured;
        }
        #endregion
    }
}
