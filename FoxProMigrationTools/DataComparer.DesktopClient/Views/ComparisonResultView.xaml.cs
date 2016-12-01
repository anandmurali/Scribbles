﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
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
using WPFLibrary.Extensions;

namespace DataComparer.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for ComparisonResultView.xaml
    /// </summary>
    public partial class ComparisonResultView : UserControl
    {
        #region Properties
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
        public ComparisonResultView(DataTable firstDbData, DataTable secondDbData)
        {
            InitializeComponent();

            DataTable firstDbMergedDataTable;
            DataTable secondDbMergedDataTable;

            DataTableComparer.MergeDataTableColumns(firstDbData, secondDbData, out firstDbMergedDataTable, out secondDbMergedDataTable);

            DataView firstDbDataView = null;
            DataView secondDbDataView = null;

            firstDbDataView = firstDbMergedDataTable.DefaultView;
            DgFirstDb.ItemsSource = firstDbDataView;

            secondDbDataView = secondDbMergedDataTable.DefaultView;
            DgSecondDb.DataContext = secondDbDataView;

            if (firstDbDataView != null && firstDbDataView.Count > 0)
                DgFirstDb.SelectedItem = firstDbDataView[0];

            if (secondDbDataView != null && secondDbDataView.Count > 0)
                DgSecondDb.SelectedItem = secondDbDataView[0];
        }
        #endregion

        #region Private Methods

        private void DbResult_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column != null && e.Column.Header != null && e.Column.Header.ToString().StartsWith("_"))
                e.Cancel = true;
        }

        private void CompareSelectedRows()
        {
            if (DgFirstDb.SelectedItem != null && DgSecondDb.SelectedItem != null)
            {
                DgResult.DataContext = DataTableComparer.Compare((DgFirstDb.SelectedItem as DataRowView).Row, (DgSecondDb.SelectedItem as DataRowView).Row);
            }
            else
            {
                DgResult.DataContext = null;
            }
        }
        #endregion

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CompareSelectedRows();
        }

        private void RadionButtonToggled(object sender, RoutedEventArgs e)
        {
            if (DgResult != null)
            {
                var temp = DgResult.DataContext;
                DgResult.DataContext = null;
                DgResult.DataContext = temp;
            }
        }

        private void DgResult_OnAutoGeneratedColumns(object sender, EventArgs e)
        {
            if (RbOnlyMismatchedData.IsChecked != null && RbOnlyMismatchedData.IsChecked.Value)
            {
                var dataTable = DgResult.DataContext as DataTable;
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    List<DataGridColumn> toRemoveList = new List<DataGridColumn>();
                    foreach (var dataGridColumn in DgResult.Columns)
                    {
                        if (dataGridColumn.Header.ToString() == Constants.DatabaseTypeColumnName)
                            continue;

                        var columnName = DynamicColumn.GetIsDataEqualColumnName(dataGridColumn.Header.ToString());
                        if (dataTable.Rows[0][columnName].ConvertToBool())
                        {
                            toRemoveList.Add(dataGridColumn);
                        }
                    }

                    foreach (var dataGridColumn in toRemoveList)
                    {
                        DgResult.Columns.Remove(dataGridColumn);
                    }
                }
            }

            if (RbAllData.IsChecked != null && RbAllData.IsChecked.Value)
            {
                foreach (var dataGridColumn in DgResult.Columns)
                {
                    dataGridColumn.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
