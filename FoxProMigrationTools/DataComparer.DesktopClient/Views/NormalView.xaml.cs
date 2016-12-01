using System;
using System.Collections.Generic;
using System.Configuration;
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
using DataComparer.Common.Domain;
using DataComparer.DesktopClient.Business;
using DataComparer.IocContainer;
using DataComparer.Common.Contracts;

namespace DataComparer.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for NormalView.xaml
    /// </summary>
    public partial class NormalView : UserControl
    {
        #region Dependency Properties

        public string DatabaseOneType
        {
            get { return (string)GetValue(DatabaseOneTypeProperty); }
            set { SetValue(DatabaseOneTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DatabaseOneType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatabaseOneTypeProperty =
            DependencyProperty.Register("DatabaseOneType", typeof(string), typeof(NormalView), new PropertyMetadata(null));



        public string DatabaseTwoType
        {
            get { return (string)GetValue(DatabaseTwoTypeProperty); }
            set { SetValue(DatabaseTwoTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DatabaseTwoType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatabaseTwoTypeProperty =
            DependencyProperty.Register("DatabaseTwoType", typeof(string), typeof(NormalView), new PropertyMetadata(null));
        #endregion

        #region Properties

        private IXmlDataProvider _xmlDataProvider;

        private IXmlDataProvider XmlDataProvider
        {
            get
            {
                if (_xmlDataProvider == null)
                    _xmlDataProvider = DataComparerContainer.Instance.GetInstance<IXmlDataProvider>();
                return _xmlDataProvider;
            }
        }

        private IDataProvider _firstDbDataProvider;

        private IDataProvider FirstDbDataProvider
        {
            get
            {
                if (_firstDbDataProvider == null)
                    _firstDbDataProvider = DataComparerContainer.Instance.GetInstance<IDataProvider>(ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseOneType]);
                return _firstDbDataProvider;
            }
        }

        private IDataProvider _secondDbDataProvider;

        private IDataProvider SecondDbDataProvider
        {
            get
            {
                if (_secondDbDataProvider == null)
                    _secondDbDataProvider = DataComparerContainer.Instance.GetInstance<IDataProvider>(ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseTwoType]);
                return _secondDbDataProvider;
            }
        }

        private IDataTableComparer _dataTableComparer;

        public IDataTableComparer DataTableComparer
        {
            get
            {
                if (_dataTableComparer == null)
                    _dataTableComparer = DataComparerContainer.Instance.GetInstance<IDataTableComparer>();
                return _dataTableComparer;
            }
        }

        #endregion

        #region Constructor
        public NormalView()
        {
            InitializeComponent();

            Initialize();
        }
        #endregion

        #region Private Methods

        private void Initialize()
        {
            var tableDetailList = XmlDataProvider.ReadFromXmlFile<TableDetailList>(Constants.XmlFilePath);
            if (tableDetailList != null)
            {
                LvTableList.ItemsSource = tableDetailList.TableDetails;
                if (tableDetailList.TableDetails != null)
                {
                    // Set Database Type
                    DatabaseOneType = ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseOneType];
                    DatabaseTwoType = ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseTwoType];
                    foreach (var tableDetail in tableDetailList.TableDetails)
                    {
                        if (tableDetail.WhereClauseConditions != null)
                        {
                            foreach (var whereClauseCondition in tableDetail.WhereClauseConditions)
                            {
                                whereClauseCondition.DatabaseOneType = DatabaseOneType;
                                whereClauseCondition.DatabaseTwoType = DatabaseTwoType;
                            }
                        }
                    }

                    // Set first item as selected item
                    LvTableList.SelectedItem = tableDetailList.TableDetails.FirstOrDefault();
                }



            }
        }
        #endregion

        #region Button Event Hanlders
        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            TableDetail tableDetail = LvTableList.SelectedItem as TableDetail;
            if (tableDetail != null)
            {
                tableDetail.FirstDatabaseData = FirstDbDataProvider.GetDataTable(Constants.AppConfigFirstDatabaseConnectionStringName, tableDetail);

                tableDetail.SecondDatabaseData = SecondDbDataProvider.GetDataTable(Constants.AppConfigSecondDatabaseConnectionStringName, tableDetail);

                tableDetail.RowComparisonList = DataTableComparer.Compare(tableDetail, tableDetail.FirstDatabaseData, tableDetail.SecondDatabaseData);
            }
        }
        #endregion

        #region DataGrid Event Handlers

        private void DbResult_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column != null && e.Column.Header != null && e.Column.Header.ToString().StartsWith("_"))
                e.Cancel = true;
        }
        #endregion

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                RowComparison rowComparison = comboBox.DataContext as RowComparison;
                TableDetail tableDetail = LvTableList.SelectedItem as TableDetail;
                if (rowComparison != null && tableDetail != null)
                {
                    //var firstDbDataRow = DataTableComparer.GetDataRow(tableDetail.FirstDatabaseData, tableDetail.PrimaryColumnName, rowComparison.FirstDatabasePrimaryColumnValue);
                    var secondDbDataRow = DataTableComparer.GetDataRow(tableDetail.SecondDatabaseData, tableDetail.PrimaryColumnName, comboBox.SelectedItem.ToString());
                    if (secondDbDataRow != null)
                    {
                        DataTableComparer.Compare(tableDetail, rowComparison, rowComparison.FirstDbDataRow, secondDbDataRow);
                    }
                }
            }
        }
    }
}
