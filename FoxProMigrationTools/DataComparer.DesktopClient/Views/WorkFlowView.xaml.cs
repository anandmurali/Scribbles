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
using DataComparer.Common.Contracts;
using DataComparer.Common.Domain;
using DataComparer.IocContainer;

namespace DataComparer.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for WorkFlowView.xaml
    /// </summary>
    public partial class WorkFlowView : UserControl
    {
        #region Dependency Properties

        public string DatabaseOneType
        {
            get { return (string)GetValue(DatabaseOneTypeProperty); }
            set { SetValue(DatabaseOneTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DatabaseOneType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatabaseOneTypeProperty =
            DependencyProperty.Register("DatabaseOneType", typeof(string), typeof(WorkFlowView), new PropertyMetadata(null));



        public string DatabaseTwoType
        {
            get { return (string)GetValue(DatabaseTwoTypeProperty); }
            set { SetValue(DatabaseTwoTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DatabaseTwoType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DatabaseTwoTypeProperty =
            DependencyProperty.Register("DatabaseTwoType", typeof(string), typeof(WorkFlowView), new PropertyMetadata(null));
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
        public WorkFlowView()
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
                // Set Database Type
                DatabaseOneType = ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseOneType];
                DatabaseTwoType = ConfigurationManager.AppSettings[Constants.AppSettingKeyDatabaseTwoType];

                foreach (var workFlow in tableDetailList.WorkFlowList)
                {
                    workFlow.TableDetailList = new List<TableDetail>();
                    foreach (var tableName in workFlow.TableNameList)
                    {
                        var tableDetail = tableDetailList.TableDetails.FirstOrDefault(td => td.TableName == tableName);
                        if (tableDetail != null)
                        {
                            workFlow.TableDetailList.Add(tableDetail);
                        }
                    }

                                    // Get unique where condition columns
                    if (workFlow.TableDetailList != null)
                    {
                        List<WhereClauseCondition> allWhereClauseConditions = new List<WhereClauseCondition>();
                        foreach (var tableDetail in workFlow.TableDetailList)
                        {
                            foreach (var whereClauseCondition in tableDetail.WhereClauseConditions)
                            {
                                var existingCondition = allWhereClauseConditions.FirstOrDefault(c => c.ColumnName == whereClauseCondition.ColumnName);
                                if (existingCondition == null)
                                    allWhereClauseConditions.Add(whereClauseCondition);
                            }
                        }
                        workFlow.WhereClauseConditions = allWhereClauseConditions;
                    }
                }



                CbWorkFlow.ItemsSource = tableDetailList.WorkFlowList;
                if (tableDetailList.WorkFlowList != null)
                    CbWorkFlow.SelectedItem = tableDetailList.WorkFlowList.FirstOrDefault();
            }
        }
        #endregion

        #region Button Event Hanlders
        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region DataGrid Event Handlers

        private void DbResult_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column != null && e.Column.Header != null && e.Column.Header.ToString().Contains("_"))
                e.Cancel = true;
        }
        #endregion
    }
}
