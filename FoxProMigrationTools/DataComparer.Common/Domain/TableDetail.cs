using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataComparer.Common.Domain
{
    [Serializable]
    public class TableDetail : DomainBase
    {
        #region Serializable Properties

        private string _tableName;

        public string TableName
        {
            get { return _tableName; }
            set
            {
                _tableName = value;
                OnPropertyChanged();
            }
        }

        private string _primaryColumnName;

        public string PrimaryColumnName
        {
            get { return _primaryColumnName; }
            set
            {
                _primaryColumnName = value;
                OnPropertyChanged();
            }
        }

        private string _primaryColumnDataType;

        public string PrimaryColumnDataType
        {
            get { return _primaryColumnDataType; }
            set
            {
                _primaryColumnDataType = value;
                OnPropertyChanged();
            }
        }

        private List<WhereClauseCondition> _whereClauseConditions;

        public List<WhereClauseCondition> WhereClauseConditions
        {
            get { return _whereClauseConditions; }
            set
            {
                _whereClauseConditions = value;
                OnPropertyChanged();
            }
        }

        private List<string> _compareColumnList;

        public List<string> CompareColumnList
        {
            get { return _compareColumnList; }
            set
            {
                _compareColumnList = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion

        #region Properties

        private DataTable _firstDatabaseData;

        [XmlIgnore]
        public DataTable FirstDatabaseData
        {
            get { return _firstDatabaseData; }
            set
            {
                _firstDatabaseData = value;
                OnPropertyChanged();
            }
        }

        private DataTable _secondDatabaseData;

        [XmlIgnore]
        public DataTable SecondDatabaseData
        {
            get { return _secondDatabaseData; }
            set
            {
                _secondDatabaseData = value;
                OnPropertyChanged();
            }
        }

        private List<RowComparison> _rowComparisonList;

        [XmlIgnore]
        public List<RowComparison> RowComparisonList
        {
            get { return _rowComparisonList; }
            set
            {
                _rowComparisonList = value;
                OnPropertyChanged();
            }
        }


        [XmlIgnore]
        public int FirstDatabaseTotalRecords
        {
            get
            {
                if (_firstDatabaseData == null)
                    return 0;
                else
                {
                    return FirstDatabaseData.Rows.Count;
                }
            }
        }

        [XmlIgnore]
        public int SecondDatabaseTotalRecords
        {
            get
            {
                if (_secondDatabaseData == null)
                    return 0;
                else
                {
                    return SecondDatabaseData.Rows.Count;
                }
            }
        }

        #endregion

        #region Constructors

        public TableDetail()
        {

        }
        #endregion
    }
}
