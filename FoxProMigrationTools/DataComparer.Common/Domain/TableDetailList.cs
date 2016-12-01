using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparer.Common.Domain
{
    [Serializable]
    public class TableDetailList : DomainBase
    {
        #region Properties

        private List<TableDetail> _tableDetails;

        public List<TableDetail> TableDetails
        {
            get { return _tableDetails; }
            set
            {
                _tableDetails = value; 
                OnPropertyChanged();
            }
        }

        private List<WorkFlow> _workFlowList;

        public List<WorkFlow> WorkFlowList
        {
            get { return _workFlowList; }
            set
            {
                _workFlowList = value;
                OnPropertyChanged();
            }
        }
        
        #endregion
    }
}
