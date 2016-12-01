using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataComparer.Common;
using DataComparer.Common.Contracts;
using DataComparer.IocContainer;

namespace DataComparer.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for RunQueryView.xaml
    /// </summary>
    public partial class RunQueryView : UserControl
    {
        public RunQueryView()
        {
            InitializeComponent();

            SetRadioButtonContent();
        }

        private void SetRadioButtonContent()
        {
            RbFirstDbOnly.Content = ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseOneType];
            RbSecondDbOnly.Content = ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseTwoType];
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            string query = string.IsNullOrEmpty(TbQuery.SelectedText) ? TbQuery.Text : TbQuery.SelectedText;
            if (RbFirstDbOnly.IsChecked != null && RbFirstDbOnly.IsChecked.Value)
            {
                var dataProvider = DataComparerContainer.Instance.GetInstance<IDataProvider>(ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseOneType]);
                contentPresenter.Visibility = Visibility.Collapsed;
                DgResultsView.Visibility = Visibility.Visible;
                DgResultsView.DataContext = dataProvider.GetDataTable(Constants.AppConfigFirstDatabaseConnectionStringName, Replace(TbReplaceString.Text, TbReplaceDb1.Text, query));
            }
            else if (RbSecondDbOnly.IsChecked != null && RbSecondDbOnly.IsChecked.Value)
            {
                var dataProvider = DataComparerContainer.Instance.GetInstance<IDataProvider>(ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseTwoType]);
                contentPresenter.Visibility = Visibility.Collapsed;
                DgResultsView.Visibility = Visibility.Visible;
                DgResultsView.DataContext = dataProvider.GetDataTable(Constants.AppConfigSecondDatabaseConnectionStringName, Replace(TbReplaceString.Text, TbReplaceDb2.Text, query));
            }
            else
            {
                //Expander.IsExpanded = false;

                var firstDbDataProvider = DataComparerContainer.Instance.GetInstance<IDataProvider>(ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseOneType]);
                DataTable firstDbData = firstDbDataProvider.GetDataTable(Constants.AppConfigFirstDatabaseConnectionStringName, Replace(TbReplaceString.Text, TbReplaceDb1.Text, query));

                var secondDbDataProvider = DataComparerContainer.Instance.GetInstance<IDataProvider>(ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseTwoType]);
                DataTable secondDbData = secondDbDataProvider.GetDataTable(Constants.AppConfigSecondDatabaseConnectionStringName, Replace(TbReplaceString.Text, TbReplaceDb2.Text, query));

                //Expander.IsExpanded = false;
                DgResultsView.Visibility = Visibility.Collapsed;
                contentPresenter.Visibility = Visibility.Visible;

                contentPresenter.Content = new ComparisonResultView(firstDbData, secondDbData);
            }

        }

        private string Replace(string key, string value, string query)
        {
            if (string.IsNullOrWhiteSpace(key))
                return query;

            return query.Replace(key, value);
        }

        private void TbQuery_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                ButtonBase_OnClick(null, null);
        }

        private void MultipleQuery_OnClick(object sender, RoutedEventArgs e)
        {
            string query = string.IsNullOrEmpty(TbQuery.SelectedText) ? TbQuery.Text : TbQuery.SelectedText;
            string db1Query = Replace(TbReplaceString.Text, TbReplaceDb1.Text, query);
            string db2Query = Replace(TbReplaceString.Text, TbReplaceDb2.Text, query);

            List<string> db1QueryList = db1Query.Replace("\r\n", "").Split(';').ToList();
            List<string> db2QueryList = db2Query.Replace("\r\n", "").Split(';').ToList();
            db1QueryList.Remove("");
            db2QueryList.Remove("");

            var firstDbDataProvider = DataComparerContainer.Instance.GetInstance<IDataProvider>(ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseOneType]);
            var secondDbDataProvider = DataComparerContainer.Instance.GetInstance<IDataProvider>(ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseTwoType]);

            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.AppConfigSecondDatabaseConnectionStringName].ConnectionString);
            OleDbConnection oleDbConnection = new OleDbConnection(ConfigurationManager.ConnectionStrings[Constants.AppConfigFirstDatabaseConnectionStringName].ConnectionString);
            cn.Open();
            oleDbConnection.Open();

            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < db1QueryList.Count; index++)
            {
                try
                {
                    DataTable firstDbData;
                    DataTable secondDbData;

                    try
                    {
                        firstDbData = firstDbDataProvider.GetResutDataTable(db1QueryList[index], oleDbConnection);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("DBF Exception " + ex.Message);
                        stringBuilder.AppendLine("-----DBF Exception-----");
                        stringBuilder.AppendLine(db1QueryList[index]);
                        stringBuilder.AppendLine(db2QueryList[index]);
                        stringBuilder.AppendLine(ex.Message);
                        continue;
                    }
                    try
                    {
                        secondDbData = secondDbDataProvider.GetResultDataTable(db2QueryList[index], cn);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SQL Exception " + ex.Message);
                        stringBuilder.AppendLine("-----SQL Exception-----");
                        stringBuilder.AppendLine(db1QueryList[index]);
                        stringBuilder.AppendLine(db2QueryList[index]);
                        stringBuilder.AppendLine(ex.Message);
                        continue;
                    }

                    if (firstDbData == null && secondDbData == null)
                    {
                        continue;
                    }

                    if (firstDbData == null || secondDbData == null)
                    {
                        stringBuilder.AppendLine("-----");
                        stringBuilder.AppendLine(db1QueryList[index]);
                        stringBuilder.AppendLine(db2QueryList[index]);
                        stringBuilder.AppendLine(GetFirstRowValue(firstDbData) + " " + GetFirstRowValue(secondDbData));
                    }

                    var firstDbValue = GetFirstRowValue(firstDbData);
                    var secondDbValue = GetFirstRowValue(secondDbData);
                    if (firstDbValue != secondDbValue)
                    {
                        stringBuilder.AppendLine("-----");
                        stringBuilder.AppendLine(db1QueryList[index]);
                        stringBuilder.AppendLine(db2QueryList[index]);
                        stringBuilder.AppendLine(GetFirstRowValue(firstDbData) + " " + GetFirstRowValue(secondDbData));
                    }
                    Console.WriteLine(index);
                }
                catch (Exception ex)
                {

                }
            }

            cn.Close();
            oleDbConnection.Close();
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Result.txt");

            File.WriteAllText(path, stringBuilder.ToString());

            Process.Start("notepad.exe", path);

        }



        private int GetFirstRowValue(DataTable dataTable)
        {
            if (dataTable != null && dataTable.Rows.Count > 0)
                return dataTable.Rows[0][0].ConvertToInt32();

            return 0;
        }

        private void TestButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
