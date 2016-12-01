using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparer.Common.Domain
{
    public class CellComparison : DomainBase
    {
        #region Properties

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

        private Type _dataType;

        public Type DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;
                OnPropertyChanged();
            }
        }
        

        private bool _isColumnAvailableInFirstDatabase;

        public bool IsColumnAvailableInFirstDatabase
        {
            get { return _isColumnAvailableInFirstDatabase; }
            set
            {
                _isColumnAvailableInFirstDatabase = value; 
                OnPropertyChanged();
            }
        }

        private bool _isColumnAvailableInSecondDatabase;

        public bool IsColumnAvailableInSecondDatabase
        {
            get { return _isColumnAvailableInSecondDatabase; }
            set
            {
                _isColumnAvailableInSecondDatabase = value; 
                OnPropertyChanged();
            }
        }

        private bool _isDataEqual;

        public bool IsDataEqual
        {
            get { return _isDataEqual; }
            set
            {
                _isDataEqual = value; 
                OnPropertyChanged();
            }
        }

        private string _firstDatabaseColumnValue;

        public string FirstDatabaseColumnValue
        {
            get { return _firstDatabaseColumnValue; }
            set
            {
                _firstDatabaseColumnValue = value; 
                OnPropertyChanged();
            }
        }

        private string _secondDatabaseColumnValue;

        public string SecondDatabaseColumnValue
        {
            get { return _secondDatabaseColumnValue; }
            set
            {
                _secondDatabaseColumnValue = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion
    }
}
