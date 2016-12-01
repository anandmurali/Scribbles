using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFPCodeConverter
{
    public class GenericConversionRuleInfo : ConversionRuleInfoBase
    {
        #region Properties

        public bool TrimLeftBeforeMatch { get; set; }

        public bool TrimRightBeforeMatch { get; set; }

        public string ReplacePattern { get; set; }

        public string Remarks { get; set; }

        #endregion
    }
}
