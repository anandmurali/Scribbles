using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WPFLibrary.Essentials;

namespace DataComparer.Common.Domain
{
    [Serializable]
    public class WorkFlow : ObservableBase
    {
        #region Serializable Properties

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value; 
                OnPropertyChanged();
            }
        }

        private List<String> _tableNameList;

        public List<String> TableNameList
        {
            get { return _tableNameList; }
            set
            {
                _tableNameList = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion

        #region Properties

        private List<TableDetail> _tableDetailList;

        [XmlIgnore]
        public List<TableDetail> TableDetailList
        {
            get { return _tableDetailList; }
            set
            {
                _tableDetailList = value; 
                OnPropertyChanged();
            }
        }

        private List<WhereClauseCondition> _whereClauseConditions;

        [XmlIgnore]
        public List<WhereClauseCondition> WhereClauseConditions
        {
            get { return _whereClauseConditions; }
            set
            {
                _whereClauseConditions = value; 
                OnPropertyChanged();
            }
        }
        
        #endregion
    }
}
