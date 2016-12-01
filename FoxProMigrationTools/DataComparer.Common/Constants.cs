using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparer.Common
{
    public class Constants
    {
        #region App.Config Constants
        public const string AppConfigFirstDatabaseConnectionStringName = "DATABASE_ONE";

        public const string AppConfigSecondDatabaseConnectionStringName = "DATABASE_TWO";
        #endregion

        #region Misc Constants
        public const string XmlFileName = "TableDetails.xml";

        public const string AppSettingKeyDatabaseOneType = "DATABASE_ONE_TYPE";

        public const string AppSettingKeyDatabaseTwoType = "DATABASE_TWO_TYPE";

        public const string AppSettingKeyTrimspaces = "TRIMSPACES";

        public const string AppSettingKeyNullDataValues = "NULL_DATE_VALUES";

        

        public const string DatabaseTypeColumnName = "DATABASE_TWO_TYPE";

        public const string Dbf = "DBF";

        public const string Sql = "SQL";
        #endregion

        #region Static Properties
        public static string XmlFilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XmlFileName);
            }
        }
        #endregion


    }
}
