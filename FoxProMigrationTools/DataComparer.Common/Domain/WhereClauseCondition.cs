using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataComparer.Common.Domain
{
    [Serializable]
    public class WhereClauseCondition : DomainBase
    {
        #region Serializable Properties

        private string _columnName;

        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                _columnName = value; 
                OnPropertyChanged();
            }
        }

        private string _columnDataType;

        public string ColumnDataType
        {
            get { return _columnDataType; }
            set
            {
                _columnDataType = value; 
                OnPropertyChanged();
            }
        }

        private string _columnValue;

        public string ColumnValue
        {
            get { return _columnValue; }
            set
            {
                _columnValue = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion

        #region Properties

        private bool _isSameValueForBothDatbase;

        [XmlIgnore]
        public bool IsSameValueForBothDatabase
        {
            get { return _isSameValueForBothDatbase; }
            set
            {
                _isSameValueForBothDatbase = value; 
                OnPropertyChanged();
            }
        }

        private string _valueForDatabaseOne;

        [XmlIgnore]
        public string ValueForDatabaseOne
        {
            get { return _valueForDatabaseOne; }
            set
            {
                _valueForDatabaseOne = value; 
                OnPropertyChanged();
            }
        }

        private string _valueForDatabaseTwo;

        [XmlIgnore]
        public string ValueForDatabaseTwo
        {
            get { return _valueForDatabaseTwo; }
            set
            {
                _valueForDatabaseTwo = value; 
                OnPropertyChanged();
            }
        }

        private string _databaseOneType;

        [XmlIgnore]
        public string DatabaseOneType
        {
            get { return _databaseOneType; }
            set
            {
                _databaseOneType = value; 
                OnPropertyChanged();
            }
        }

        private string _databaseTwoType;

        [XmlIgnore]
        public string DatabaseTwoType
        {
            get { return _databaseTwoType; }
            set
            {
                _databaseTwoType = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion

        #region Constructors

        public WhereClauseCondition()
        {
            IsSameValueForBothDatabase = true;
        }
        #endregion
    }
}
