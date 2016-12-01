using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFPCodeConverter.DTOs;

namespace VFPCodeConverter.CodeBuilders
{
    public class MethodBuilderBase
    {
        #region Properties

        public string LeadingSpace { get; set; }

        public string MethodName { get; set; }

        public string InLineComments { get; set; }

        public List<MethodParameterInfo> Parameters { get; set; }

        public List<string> LinesOfCode { get; set; }
        #endregion

        #region Constructor

        public MethodBuilderBase()
        {
            Parameters = new List<MethodParameterInfo>();

            LinesOfCode = new List<string>();
        }
        #endregion

        #region Pubilc Methods
        #endregion

    }
}
