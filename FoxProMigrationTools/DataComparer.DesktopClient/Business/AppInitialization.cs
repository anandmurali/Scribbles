using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataComparer.Common;
using DataComparer.Common.Domain;
using DataComparer.Common.Contracts;

namespace DataComparer.DesktopClient.Business
{
    public class AppInitialization
    {
        #region Fields

        private readonly IXmlDataProvider _tableDetailXmlDataProvider;

        #endregion

        #region Constructors

        public AppInitialization(IXmlDataProvider tableDetailXmlDataProvider)
        {
            _tableDetailXmlDataProvider = tableDetailXmlDataProvider;
        }
        #endregion

        #region Public Methods

        public TableDetail GetTableDetailFromXmlFile()
        {
            return _tableDetailXmlDataProvider.ReadFromXmlFile<TableDetail>(Constants.XmlFilePath);
        }
        #endregion
    }
}
