using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparer.Common.Contracts
{
    public interface ITypeResolver
    {
        Type ResolveType(string strType);
    }
}
