using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFPCodeConverter.DTOs
{
    public class MethodParameterInfo
    {
        #region Properties

        public string Name { get; set; }

        public string DataType { get; set; }

        public bool IsOptional { get; set; }

        public object OptionalValue { get; set; }

        #endregion
    }
}
