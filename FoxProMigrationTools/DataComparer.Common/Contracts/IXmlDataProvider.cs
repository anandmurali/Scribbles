using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparer.Common.Contracts
{
    public interface IXmlDataProvider
    {
        void SaveToXmlFile<T>(string filePath, T instanceToSave)  where T : class;

        T ReadFromXmlFile<T>(string filePath)  where T : class;
    }
}
