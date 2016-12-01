using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataComparer.Common.Domain;

namespace DataComparer.Dal
{
    public abstract class DataProviderBase
    {
        #region Protected Methods

        protected bool IsArgumentsValid(string appSetttingsConnectionString, TableDetail tableDetail)
        {
            if (string.IsNullOrEmpty(appSetttingsConnectionString))
                throw new ArgumentException("The application config connection string name cannot be null or empty");

            if (tableDetail == null)
                throw new ArgumentException("TableDetail cannot be null");

            return true;
        }

        protected string GetWhereClause(List<WhereClauseCondition> whereClauseConditions, bool isForFirstDatabase)
        {
            if (whereClauseConditions != null && whereClauseConditions.Count > 0)
            {
                string whereClauseCondition = " WHERE ";
                int loopCount = 0;
                foreach (var filterCondition in whereClauseConditions)
                {
                    var columnValue = (filterCondition.IsSameValueForBothDatabase ? filterCondition.ColumnValue : (isForFirstDatabase ? filterCondition.ValueForDatabaseOne : filterCondition.ValueForDatabaseTwo));
                    if (loopCount == 0)
                        whereClauseCondition = whereClauseCondition + filterCondition.ColumnName + "='" + columnValue + "'";
                    else
                        whereClauseCondition = whereClauseCondition + " AND " + filterCondition.ColumnName + "='" + columnValue + "'";

                    loopCount++;
                }
                return whereClauseCondition;
            }
            return string.Empty;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
