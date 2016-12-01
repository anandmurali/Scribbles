using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataComparer.Common.Contracts;

namespace DataComparer.Dal
{
    public class TypeResolver : ITypeResolver
    {
        #region ITypeResolver Implementation

        public Type ResolveType(string strType)
        {
            if (string.IsNullOrEmpty(strType))
                return null;

            switch (strType.ToLower())
            {
                case "bit":
                    return typeof (bool);
                case "numeric":
                case "decimal":
                    return typeof (double);
                    break;
                case "string":
                    return typeof (string);
                    break;
                default:
                    return typeof (string);
                    break;
            }

        }
        #endregion
    }
}
