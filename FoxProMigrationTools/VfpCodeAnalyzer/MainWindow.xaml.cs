using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
using VfpCodeAnalyzer.Properties;

namespace VfpCodeAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ProjectDetail ProjectDetail { get; set; }

        public CodeSearchOptions CodeSearchOptions { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoButton_OnClick(object sender, RoutedEventArgs e)
        {
            ReadProjectData();

            CodeSearchOptions.SearchString = SearchText.Text;

            var result = CodeSearchOptions.FileDetails.FirstOrDefault(file => file.IsIncluded);
            if (result != null)
                CodeSearchOptions.SearchInSelectedFiles = true;
            else
                CodeSearchOptions.SearchInSelectedFiles = false;

            var codeSearch = new CodeSearch();
            codeSearch.Search(ProjectDetail, CodeSearchOptions);

            ResultDataGrid.DataContext = codeSearch.ResultsDataTable;
        }

        private void ReadProjectData()
        {
            string path = ConfigurationManager.AppSettings["PJX_PATH"];
            //string path = @"D:\WCP_Files\WCP VFP Project 2014-06-30\WCP\WCP.pjx";

            if (ProjectDetail == null)
            {
                var codeReader = new CodeReader();
                codeReader.ReadProject(path);

                ProjectDetail = codeReader.ProjectDetail;
            }

            if (CodeSearchOptions == null)
            {
                FillCodeSearchOptions();
            }
        }

        private void FillCodeSearchOptions()
        {
            CodeSearchOptions searchOptions = new CodeSearchOptions();
            searchOptions.SearchString = SearchText.Text;
            searchOptions.FileDetails = new List<SearchFileDetail>();

            foreach (DataRow dataRow in ProjectDetail.FileNamesDataTable.Rows)
            {
                SearchFileDetail searchFileDetail = new SearchFileDetail();
                searchFileDetail.FileName = System.IO.Path.GetFileName(dataRow["name"].ConvertToString());
                searchFileDetail.FilePath = dataRow["name"].ConvertToString();

                searchOptions.FileDetails.Add(searchFileDetail);
            }
            searchOptions.FileDetails = searchOptions.FileDetails.OrderBy(s => s.FileName).ToList();

            CodeSearchOptions = searchOptions;
            SearchOptionsExpander.DataContext = CodeSearchOptions;
        }

        private void ColComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gridSelectedItem = ResultDataGrid.SelectedItem as DataRowView;

            if (gridSelectedItem != null)
            {
                int noOfOccurrences = 0;
                RichTextBox.Document = GetFlowDoucment(gridSelectedItem["Contents"].ToString(), SearchText.Text, out noOfOccurrences);

                TotalCountTextBlock.Text = " " + noOfOccurrences;
            }
        }

        private FlowDocument GetFlowDoucment(string contents, string highlightText, out int noOfOccurrences)
        {
            FlowDocument flowDocument = new FlowDocument();

            Paragraph paragraph = new Paragraph();
            //paragraph.Inlines.Add(contents);
            //paragraph.Inlines.Add("\n\n\n\n###########################################\n\n\n\n");

            int runningIndex = 0;
            noOfOccurrences = 0;
            foreach (int index in AllIndexesOf(contents, highlightText))
            {
                paragraph.Inlines.Add(contents.Substring(runningIndex, index - runningIndex));

                Run run = new Run(contents.Substring(index, highlightText.Length));
                run.Background = Brushes.Yellow;
                paragraph.Inlines.Add(run);

                runningIndex = index + highlightText.Length;

                noOfOccurrences++;
            }
            paragraph.Inlines.Add(contents.Substring(runningIndex, contents.Length - runningIndex));

            flowDocument.Blocks.Add(paragraph);

            return flowDocument;
        }

        public static IEnumerable<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException();

            str = str.ToLower();
            value = value.ToLower();

            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.Ordinal);
                if (index == -1)
                    break;
                yield return index;
            }
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            ReadProjectData();

            FillCodeSearchOptions();

            UpdateSelectedCount();
        }

        private void Reset_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var searchFileDetail in CodeSearchOptions.FileDetails)
            {
                searchFileDetail.IsIncluded = false;
            }
            UpdateSelectedCount();
        }

        private void FileDetailsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
        {
            int selectedCount = 0;
            foreach (var searchFileDetail in CodeSearchOptions.FileDetails)
            {
                if (searchFileDetail.IsIncluded)
                {
                    selectedCount++;
                }
            }
            SelectedFileCount.Text = " " + selectedCount + " ";
        }
    }
}
