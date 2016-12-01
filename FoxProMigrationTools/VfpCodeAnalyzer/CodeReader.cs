using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VfpCodeAnalyzer
{
    public class CodeReader
    {
        private static readonly string ColName = "name";

        #region Properties

        public ProjectDetail ProjectDetail { get; set; }
        #endregion

        #region Public Static Methods

        /// <summary>
        /// Reads the project.
        /// </summary>
        /// <param name="projectPath">The project path (.pjx file).</param>
        public void ReadProject(string projectPath)
        {
            try
            {
                ProjectDetail = new ProjectDetail();

                ProjectDetail.ProjectPath = Path.GetFullPath(projectPath);
                ProjectDetail.ProjectName = Path.GetFileName(projectPath);
                ProjectDetail.ProjectDirectory = Path.GetDirectoryName(projectPath);

                FillFileNamesDataTable();

                FillFileDetailsDataTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                System.Diagnostics.Debugger.Break();
            }

        }
        #endregion

        #region Private Static Methods

        /// <summary>
        /// Fills the file names data table.
        /// </summary>
        private void FillFileNamesDataTable()
        {
            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder();
            oleDbConnectionStringBuilder.Provider = "VFPOLEDB";
            oleDbConnectionStringBuilder.DataSource = ProjectDetail.ProjectPath;
            using (OleDbConnection oleDbConnection = new OleDbConnection(oleDbConnectionStringBuilder.ConnectionString))
            {
                oleDbConnection.Open();

                string query = "SELECT NAME, TYPE FROM '{0}' WHERE INLIST(TYPE, 'P', 'M', 'V', 'T', 'K', 'R') ORDER BY TYPE";

                OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(
                   string.Format(query, ProjectDetail.ProjectName), oleDbConnection);

                oleDbDataAdapter.Fill(ProjectDetail.FileNamesDataTable);
                TempFill(ProjectDetail.FileNamesDataTable);
            }
        }

        private void TempFill(DataTable dataTable)
        {
            List<string> scxFiles = new List<string>();
            scxFiles.Add(@"forms\fi\transacecexport.scx");
            scxFiles.Add(@"forms\fi\transacexport.scx");
            scxFiles.Add(@"forms\fi\transacreevalimport.scx");
            scxFiles.Add(@"forms\fi\margincallnew.scx");

            foreach (var scxFile in scxFiles)
            {
                bool isFound = false;
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row[0].ConvertToString().ToLower() == scxFile)
                        isFound = true;
                }

                if (isFound == false)
                {
                    var dataRow = dataTable.NewRow();
                    dataRow[0] = scxFile;
                    dataRow[1] = "K";
                    dataTable.Rows.Add(dataRow);
                }
            }
        }


        /// <summary>
        /// Fills the file details data table.
        /// </summary>
        private void FillFileDetailsDataTable()
        {
            foreach (DataRow dataRow in ProjectDetail.FileNamesDataTable.Rows)
            {
                switch (dataRow["type"].ToString())
                {
                    case "P":
                        ProcessProgram(dataRow[ColName].ToString());
                        break;
                    case "M":
                        ProcessMenu(dataRow[ColName].ToString());
                        break;
                    case "V":
                        ProcessClassLibrary(dataRow[ColName].ToString());
                        break;
                    case "K":
                        ProcessForm(dataRow[ColName].ToString());
                        break;
                    case "T":
                        ProcessInclude(dataRow[ColName].ToString());
                        break;
                    case "R":
                        ProcessReport(dataRow[ColName].ToString());
                        break;
                }
            }
        }

        /// <summary>
        /// Reads .prj files.
        /// </summary>
        /// <param name="prjRelativePath">The .prj relative path.</param>
        private void ProcessProgram(string prjRelativePath)
        {
            string prgFullPath = Path.Combine(ProjectDetail.ProjectDirectory, prjRelativePath);

            var dataRow = ProjectDetail.LibCodeDataTable.NewRow();

            dataRow[ProjectDetail.ColFileName] = Path.GetFileName(prjRelativePath);
            dataRow[ProjectDetail.ColFilePath] = prgFullPath;
            dataRow[ProjectDetail.ColContents] = File.ReadAllText(prgFullPath);

            ProjectDetail.LibCodeDataTable.Rows.Add(dataRow);
        }

        /// <summary>
        /// Reads a Menu (.MNX) file
        /// </summary>
        /// <param name="mnxRelativePath">Path and file name of .MNX file to analyze</param>
        private void ProcessMenu(string mnxRelativePath)
        {
            string mnxFullPath = Path.Combine(ProjectDetail.ProjectDirectory, mnxRelativePath);
            string mnxName = Path.GetFileName(mnxRelativePath);

            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder();
            oleDbConnectionStringBuilder.Provider = "VFPOLEDB";
            oleDbConnectionStringBuilder.DataSource = mnxFullPath;
            using (OleDbConnection oleDbConnection = new OleDbConnection(oleDbConnectionStringBuilder.ConnectionString))
            {

                oleDbConnection.Open();

                string query = "SELECT * FROM '{0}' WHERE OBJTYPE = 3";
                OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(string.Format(query, mnxName), oleDbConnection);

                DataTable mnxDataTable = new DataTable();
                oleDbDataAdapter.Fill(mnxDataTable);
                ProjectDetail.MergeMenuCodeDataTable(mnxDataTable);
            }
        }

        /// <summary>
        /// Reads Class Library .VCX file
        /// </summary>
        /// <param name="vcxRelativePath">Relative path of .VCX file</param>
        private void ProcessClassLibrary(string vcxRelativePath)
        {
            string vcxFullPath = Path.Combine(ProjectDetail.ProjectDirectory, vcxRelativePath);
            string vcxName = Path.GetFileName(vcxRelativePath);

            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder();
            oleDbConnectionStringBuilder.Provider = "VFPOLEDB";
            oleDbConnectionStringBuilder.DataSource = vcxFullPath;
            using (OleDbConnection oleDbConnection = new OleDbConnection(oleDbConnectionStringBuilder.ConnectionString))
            {

                oleDbConnection.Open();

                OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM '" + vcxName + "'", oleDbConnection);

                DataTable vcxDataTable = new DataTable();
                oleDbDataAdapter.Fill(vcxDataTable);
                ProjectDetail.MergeFormCodeDataTable(vcxDataTable, vcxFullPath);
            }
        }

        /// <summary>
        /// Reads a Form (.SCX) file
        /// </summary>
        /// <param name="scxRelativePath">Relative path of .SCX file</param>
        private void ProcessForm(string scxRelativePath)
        {
            string scxFullPath = Path.Combine(ProjectDetail.ProjectDirectory, scxRelativePath);
            string scxName = Path.GetFileName(scxRelativePath);

            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder();
            oleDbConnectionStringBuilder.Provider = "VFPOLEDB";
            oleDbConnectionStringBuilder.DataSource = scxFullPath;
            using (OleDbConnection oleDbConnection = new OleDbConnection(oleDbConnectionStringBuilder.ConnectionString))
            {

                oleDbConnection.Open();

                OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM '" + scxName + "'", oleDbConnection);

                DataTable scxDataTable = new DataTable();
                oleDbDataAdapter.Fill(scxDataTable);
                ProjectDetail.MergeFormCodeDataTable(scxDataTable, scxFullPath);
            }
        }

        /// <summary>
        /// Reads a .H include file
        /// </summary>
        /// <param name="hFileRelativePath">Relative path of .H file</param>
        private void ProcessInclude(string hFileRelativePath)
        {
            string hFileFullPath = Path.Combine(ProjectDetail.ProjectDirectory, hFileRelativePath);

            var dataRow = ProjectDetail.LibCodeDataTable.NewRow();

            dataRow[ProjectDetail.ColFileName] = Path.GetFileName(hFileRelativePath);
            dataRow[ProjectDetail.ColFilePath] = hFileFullPath;
            dataRow[ProjectDetail.ColContents] = File.ReadAllText(hFileFullPath);

            ProjectDetail.LibCodeDataTable.Rows.Add(dataRow);
        }

        /// <summary>
        /// Reads a .FRX report file
        /// </summary>
        /// <param name="frxRelativePath">Relative path of .FRX file</param>
        private void ProcessReport(string frxRelativePath)
        {
            string frxFullPath = Path.Combine(ProjectDetail.ProjectDirectory, frxRelativePath);
            string frxName = Path.GetFileName(frxRelativePath);

            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder();
            oleDbConnectionStringBuilder.Provider = "VFPOLEDB";
            oleDbConnectionStringBuilder.DataSource = frxFullPath;
            using (OleDbConnection oleDbConnection = new OleDbConnection(oleDbConnectionStringBuilder.ConnectionString))
            {
                try
                {
                    oleDbConnection.Open();

                    OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM '" + frxName + "' WHERE OBJTYPE = 8", oleDbConnection);

                    DataTable frxDataTable = new DataTable();
                    oleDbDataAdapter.Fill(frxDataTable);
                    ProjectDetail.MergeReportCodeDataTable(frxDataTable);
                }
                catch (Exception ex)
                {
                    ProjectDetail.AddReportError(frxFullPath, ex);
                }
            }

        }
        #endregion
    }
}
