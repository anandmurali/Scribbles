using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using DataComparer.Common;
using DataComparer.Common.Contracts;
using DataComparer.Common.Domain;
using DataComparer.IocContainer;

namespace DataComparer.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
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
        #endregion

        #region Constructors
        public MainView()
        {
            CreateSampleFile();

            InitializeComponent();

        }

        #endregion

        #region Private Methods
        private void CreateSampleFile()
        {
            TableDetailList tableDetailList = new TableDetailList();
            tableDetailList.TableDetails = new List<TableDetail>();
            TableDetail tableDetail = new TableDetail();
            tableDetail.TableName = "FIVOUCHERS";
            tableDetail.PrimaryColumnName = "VOUCHERIDS";
            tableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            tableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "VOUCHERIDS", ColumnDataType = "string" });
            tableDetail.CompareColumnList = new List<string>() { "VOUCHERIDS" };

            TableDetail gnJouTableDetail = new TableDetail();
            gnJouTableDetail.TableName = "GNJOU";
            gnJouTableDetail.PrimaryColumnName = "IDS";
            gnJouTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            gnJouTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "PARENTIDS", ColumnDataType = "string" });
            gnJouTableDetail.CompareColumnList = new List<string>() { "CPTNOS", "CPTOPS", "DBCRC", "Operators", "Statutc" };

            TableDetail payOrdersTableDetail = new TableDetail();
            payOrdersTableDetail.TableName = "PMTORDERS";
            payOrdersTableDetail.PrimaryColumnName = "IDS";
            payOrdersTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            payOrdersTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "PARENTIDS", ColumnDataType = "string" });
            payOrdersTableDetail.CompareColumnList = new List<string>() { "PARENTIDS" };

            TableDetail fiForexTableDetail = new TableDetail();
            fiForexTableDetail.TableName = "FIFOREX";
            fiForexTableDetail.PrimaryColumnName = "VOUCHERIDS";
            fiForexTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            fiForexTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "VOUCHERIDS", ColumnDataType = "string" });
            fiForexTableDetail.CompareColumnList = new List<string>() { "VOUCHERIDS" };

            TableDetail swiftFxOrdersTableDetail = new TableDetail();
            swiftFxOrdersTableDetail.TableName = "SWIFTFXORDERS";
            swiftFxOrdersTableDetail.PrimaryColumnName = "SWIFTORDERIDS";
            swiftFxOrdersTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            swiftFxOrdersTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "VOUCHERIDS", ColumnDataType = "string" });
            swiftFxOrdersTableDetail.CompareColumnList = new List<string>() { "VOUCHERIDS" };

            TableDetail fiMomaTableDetail = new TableDetail();
            fiMomaTableDetail.TableName = "FIMOMA";
            fiMomaTableDetail.PrimaryColumnName = "VOUCHERIDS";
            fiMomaTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            fiMomaTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "VOUCHERIDS", ColumnDataType = "string" });
            fiMomaTableDetail.CompareColumnList = new List<string>() { "VOUCHERIDS" };

            TableDetail fiMomoaEleTableDetail = new TableDetail();
            fiMomoaEleTableDetail.TableName = "FIMOMAELE";
            fiMomoaEleTableDetail.PrimaryColumnName = "VOUCHERIDS";
            fiMomoaEleTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            fiMomoaEleTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "VOUCHERIDS", ColumnDataType = "string" });
            fiMomoaEleTableDetail.CompareColumnList = new List<string>() { "VOUCHERIDS" };

            TableDetail fiSwapTableDetail = new TableDetail();
            fiSwapTableDetail.TableName = "FISWAP";
            fiSwapTableDetail.PrimaryColumnName = "VOUCHERIDS";
            fiSwapTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            fiSwapTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "VOUCHERIDS", ColumnDataType = "string" });
            fiSwapTableDetail.CompareColumnList = new List<string>() { "VOUCHERIDS" };

            TableDetail fiSwapEleTableDetail = new TableDetail();
            fiSwapEleTableDetail.TableName = "FISWAPELE";
            fiSwapEleTableDetail.PrimaryColumnName = "VOUCHERIDS";
            fiSwapEleTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            fiSwapEleTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "VOUCHERIDS", ColumnDataType = "string" });
            fiSwapEleTableDetail.CompareColumnList = new List<string>() { "VOUCHERIDS", "CALCDATED", "FIXINGDATED", "INOUTC" };

            TableDetail lbaTypeDocTableDetail = new TableDetail();
            lbaTypeDocTableDetail.TableName = "LBATYPEDOCS";
            lbaTypeDocTableDetail.PrimaryColumnName = "CAPTIONS";
            lbaTypeDocTableDetail.WhereClauseConditions = new List<WhereClauseCondition>();
            lbaTypeDocTableDetail.WhereClauseConditions.Add(new WhereClauseCondition() { ColumnName = "CAPTIONS", ColumnDataType = "string" });

            tableDetailList.TableDetails.Add(tableDetail);
            tableDetailList.TableDetails.Add(gnJouTableDetail);
            tableDetailList.TableDetails.Add(payOrdersTableDetail);
            tableDetailList.TableDetails.Add(fiForexTableDetail);
            tableDetailList.TableDetails.Add(swiftFxOrdersTableDetail);
            tableDetailList.TableDetails.Add(fiMomaTableDetail);
            tableDetailList.TableDetails.Add(fiMomoaEleTableDetail);
            tableDetailList.TableDetails.Add(fiSwapTableDetail);
            tableDetailList.TableDetails.Add(fiSwapEleTableDetail);
            tableDetailList.TableDetails.Add(lbaTypeDocTableDetail);

            tableDetailList.WorkFlowList = new List<WorkFlow>();
            WorkFlow workFlow = new WorkFlow();
            workFlow.Name = "IRS";
            workFlow.TableNameList = new List<string>();
            workFlow.TableNameList.Add("FIVOUCHERS");
            workFlow.TableNameList.Add("FISWAP");
            workFlow.TableNameList.Add("FISWAPELE");
            workFlow.TableNameList.Add("GNJOU");
            workFlow.TableNameList.Add("PMTORDERS");

            tableDetailList.WorkFlowList.Add(workFlow);


            XmlDataProvider.SaveToXmlFile(Constants.XmlFilePath, tableDetailList);
        }
        #endregion

        #region Menu Items
        private void MiExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
