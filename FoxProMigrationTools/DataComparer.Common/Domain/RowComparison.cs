using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparer.Common.Domain
{
    public class RowComparison : DomainBase
    {
        #region Properties

        private DataRow _firstDbDataRow;

        public DataRow FirstDbDataRow
        {
            get { return _firstDbDataRow; }
            set
            {
                _firstDbDataRow = value; 
                OnPropertyChanged();
            }
        }
        

        private string _firstDatabasePrimaryColumnValue;

        public string FirstDatabasePrimaryColumnValue
        {
            get { return _firstDatabasePrimaryColumnValue; }
            set
            {
                _firstDatabasePrimaryColumnValue = value; 
                OnPropertyChanged();
            }
        }

        private string _secondDatabasePrimaryColumnValue;

        public string SecondDatabasePrimaryColumnValue
        {
            get { return _secondDatabasePrimaryColumnValue; }
            set
            {
                _secondDatabasePrimaryColumnValue = value; 
                OnPropertyChanged();
            }
        }

        private List<string> _secondDatabasePrimaryColumnValueList;

        public List<string> SecondDatabasePrimaryColumnValueList
        {
            get { return _secondDatabasePrimaryColumnValueList; }
            set
            {
                _secondDatabasePrimaryColumnValueList = value; 
                OnPropertyChanged();
            }
        }

        private DataTable _resultDataTable;

        public DataTable ResultDataTable
        {
            get { return _resultDataTable; }
            set
            {
                _resultDataTable = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion
    }
}
